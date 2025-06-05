using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Application.DTOs;
using Core.Entities;
using Duende.IdentityModel.Client;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new ApplicationUser { UserName = registerDto.Username, Email = registerDto.Email };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
                return Ok(new { message = "Register success" });
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var authority = _config["IdentityServer:Authority"];
                using var http = new HttpClient();
                var disco = await http.GetDiscoveryDocumentAsync(authority);
                var tokenResponse = await http.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "my-client",
                    ClientSecret = "secret",
                    UserName = loginDto.Username,
                    Password = loginDto.Password,
                    Scope = "api1 openid profile"
                });

                if (tokenResponse.IsError)
                    return Unauthorized(new { message = tokenResponse.Error });

                return Ok(new
                {
                    access_token = tokenResponse.AccessToken,
                    expires_in = tokenResponse.ExpiresIn,
                    token_type = tokenResponse.TokenType,
                    scope = tokenResponse.Scope
                });
            }

            if (result.IsLockedOut)
                return Unauthorized(new { message = "Account locked. Please try again later." });

            return Unauthorized(new { message = "Invalid username or password" });
        }
    }
}