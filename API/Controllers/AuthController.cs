using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Application.DTOs;
using Core.Entities;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout success" });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
        {
            var authority = _config["IdentityServer:Authority"];
            using var http = new HttpClient();
            var disco = await http.GetDiscoveryDocumentAsync(authority);
            if (disco.IsError) return StatusCode(500, disco.Error);

            var revokeResponse = await http.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = disco.RevocationEndpoint,
                ClientId = "my-client",
                ClientSecret = "secret",
                Token = dto.Token, // รับ refresh token หรือ access token จาก client
                TokenTypeHint = dto.TokenType // "refresh_token" หรือ "access_token"
            });

            if (revokeResponse.IsError)
                return BadRequest(new { message = revokeResponse.Error });

            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            var authority = _config["IdentityServer:Authority"];
            using var http = new HttpClient();
            var disco = await http.GetDiscoveryDocumentAsync(authority);
            if (disco.IsError) return StatusCode(500, disco.Error);

            var tokenResponse = await http.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "my-client",
                ClientSecret = "secret",
                RefreshToken = dto.RefreshToken
            });

            if (tokenResponse.IsError)
                return BadRequest(new { message = tokenResponse.Error });

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                expires_in = tokenResponse.ExpiresIn,
                token_type = tokenResponse.TokenType,
                scope = tokenResponse.Scope
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Ok(new { message = "If the email exists, a reset link has been sent." }); // ป้องกัน brute-force

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{_config["App:BaseUrl"]}/reset-password?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(token)}";

            // TODO: ส่งอีเมล resetLink ไปยัง user.Email (ใช้ EmailService หรือ SMTP)
            // ตัวอย่าง: await _emailSender.SendAsync(user.Email, "Reset Password", $"Reset link: {resetLink}");

            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (result.Succeeded) return Ok(new { message = "Password changed" });
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(new { user.UserName, user.Email });
        }
    }
}