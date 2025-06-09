using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result.Succeeded)
                return Ok(new { message = "Register success" });
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var tokenResponse = await _authService.LoginAsync(loginDto);
            if (tokenResponse.IsError)
                return Unauthorized(new { message = tokenResponse.Error });
            return Ok(
                new
                {
                    access_token = tokenResponse.AccessToken,
                    expires_in = tokenResponse.ExpiresIn,
                    token_type = tokenResponse.TokenType,
                    scope = tokenResponse.Scope,
                }
            );
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Logout success" });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
        {
            var revokeResponse = await _authService.RevokeTokenAsync(dto);
            if (revokeResponse.IsError)
                return BadRequest(new { message = revokeResponse.Error });
            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            var tokenResponse = await _authService.RefreshTokenAsync(dto);
            if (tokenResponse.IsError)
                return BadRequest(new { message = tokenResponse.Error });
            return Ok(
                new
                {
                    access_token = tokenResponse.AccessToken,
                    expires_in = tokenResponse.ExpiresIn,
                    token_type = tokenResponse.TokenType,
                    scope = tokenResponse.Scope,
                }
            );
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _authService.ChangePasswordAsync(userId, dto);
            if (result.Succeeded)
                return Ok(new { message = "Password changed" });
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // สมมติว่ามี GetMeAsync ใน IAuthService
            var userInfo = await _authService.GetMeAsync(User);
            return Ok(userInfo);
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto externalLoginDto)
        {
            var loginResponse = await _authService.ExternalLoginAsync(externalLoginDto);
            if (loginResponse.IsError)
                return BadRequest(new { message = loginResponse.Error });
            return Ok(
                new
                {
                    access_token = loginResponse.AccessToken,
                    expires_in = loginResponse.ExpiresIn,
                    token_type = loginResponse.TokenType,
                    scope = loginResponse.Scope,
                }
            );
        }
    }
}