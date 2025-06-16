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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred during registration for user {Username}",
                    registerDto.Username
                );
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var tokenResponse = await _authService.LoginAsync(loginDto);
                if (tokenResponse.IsError)
                {
                    _logger.LogWarning(
                        "Login failed for user {Username}: {Error}",
                        loginDto.Username,
                        tokenResponse.Error
                    );
                    // Always use a generic message for invalid credentials
                    return Unauthorized(new { message = "Authentication failed" });
                }
                if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    Response.Cookies.Append(
                        "access_token",
                        tokenResponse.AccessToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, // ต้องเป็น true เมื่อ SameSite=None
                            SameSite = SameSiteMode.None, // ต้องใช้ None ถ้าจะ cross-site
                            Path = "/", // กำหนด path ที่ cookie นี้จะถูกส่งไป
                            Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                        }
                    );
                }
                // ไม่จำเป็นต้อง return access_token ใน body แล้ว
                return Ok(
                    new
                    {
                        message = "Login success",
                        access_token = tokenResponse.AccessToken,
                        refresh_token = tokenResponse.RefreshToken,
                        expires_in = tokenResponse.ExpiresIn,
                        token_type = tokenResponse.TokenType,
                        scope = tokenResponse.Scope,
                    }
                );
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Login failed: Authentication failed");
                // Always use a generic message for invalid credentials
                return Unauthorized(new { message = "Authentication failed" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authService.LogoutAsync();
                _logger.LogInformation("User logged out successfully");
                return Ok(new { message = "Logout success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout");
                return StatusCode(500, new { message = "An error occurred during logout." });
            }
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
        {
            try
            {
                var revokeResponse = await _authService.RevokeTokenAsync(dto);
                if (revokeResponse.IsError)
                    return BadRequest(new { message = revokeResponse.Error });

                return Ok(new { message = "Token revoked" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token revocation");
                return StatusCode(
                    500,
                    new { message = "An error occurred during token revocation." }
                );
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            try
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
                        refresh_token = tokenResponse.RefreshToken,
                        expires_in = tokenResponse.ExpiresIn,
                        token_type = tokenResponse.TokenType,
                        scope = tokenResponse.Scope,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh");
                return StatusCode(500, new { message = "An error occurred during token refresh." });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email))
                {
                    _logger.LogWarning("Forgot password request with empty email");
                    return BadRequest(new { message = "Email is required" });
                }

                await _authService.ForgotPasswordAsync(dto);
                _logger.LogInformation("Password reset link sent to {Email}", dto.Email);

                return Ok(new { message = "If the email exists, a reset link has been sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during forgot password request");
                return StatusCode(
                    500,
                    new { message = "An error occurred during forgot password." }
                );
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.NewPassword))
                {
                    _logger.LogWarning("Change password request with invalid data");
                    return BadRequest(new { message = "Invalid data" });
                }

                var userId = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during change password request");
                return StatusCode(
                    500,
                    new { message = "An error occurred during change password." }
                );
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthorized access to user information");
                    return Unauthorized(new { message = "Unauthorized" });
                }

                // สมมติว่ามี GetMeAsync ใน IAuthService
                var userId = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;
                var userInfo = await _authService.GetMeAsync(User);
                _logger.LogInformation("Retrieved user information for user {UserId}", userId);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while accessing user information");
                return StatusCode(
                    500,
                    new { message = "An error occurred while accessing user information." }
                );
            }
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto externalLoginDto)
        {
            try
            {
                if (externalLoginDto == null || string.IsNullOrEmpty(externalLoginDto.Provider))
                {
                    _logger.LogWarning("External login request with invalid data");
                    return BadRequest(new { message = "Invalid external login data" });
                }

                _logger.LogInformation(
                    "Attempting external login for provider {Provider}",
                    externalLoginDto.Provider
                );

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during external login request");
                return StatusCode(
                    500,
                    new { message = "An error occurred during external login." }
                );
            }
        }
    }
}
