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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "User {Username} registered successfully",
                    registerDto.Username
                );
                return Ok(new { message = "Register success" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var tokenResponse = await _authService.LoginAsync(loginDto);
            if (tokenResponse.IsError)
            {
                _logger.LogWarning(
                    "Login failed for user {Username}: {Error}",
                    loginDto.Username,
                    tokenResponse.Error
                );
                return Unauthorized(new { message = tokenResponse.Error });
            }
            if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                var userId = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;
                _logger.LogInformation("User {UserId} logged in successfully", userId);
                _logger.LogInformation("Access token: {AccessToken}", tokenResponse.AccessToken);
                _logger.LogInformation(
                    "Token expires in: {ExpiresIn} seconds",
                    tokenResponse.ExpiresIn
                );
                _logger.LogInformation("Token type: {TokenType}", tokenResponse.TokenType);
                _logger.LogInformation("Token scope: {Scope}", tokenResponse.Scope);
            }

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
            _logger.LogInformation("User logged out successfully");
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
            {
                _logger.LogWarning("Token refresh failed: {Error}", tokenResponse.Error);
                return BadRequest(new { message = tokenResponse.Error });
            }
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
            _logger.LogInformation("Password reset link sent to {Email}", dto.Email);

            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _authService.ChangePasswordAsync(userId, dto);
            if (result == null)
            {
                _logger.LogWarning("Change password failed for user {UserId}", userId);
                return BadRequest(new { message = "User not found" });
            }
            if (result.Succeeded)
                return Ok(new { message = "Password changed" });
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // สมมติว่ามี GetMeAsync ใน IAuthService
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userInfo = await _authService.GetMeAsync(User);
            _logger.LogInformation("Retrieved user information for user {UserId}", userId);
            return Ok(userInfo);
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto externalLoginDto)
        {
            var loginResponse = await _authService.ExternalLoginAsync(externalLoginDto);
            if (loginResponse == null)
            {
                _logger.LogWarning(
                    "External login failed for provider {Provider}",
                    externalLoginDto.Provider
                );
                return BadRequest(new { message = "External login failed" });
            }
            if (loginResponse.IsError)
                return BadRequest(new { message = loginResponse.Error });

            _logger.LogInformation("External user logged in successfully");
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
