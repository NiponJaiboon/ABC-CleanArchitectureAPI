using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        // สมัครสมาชิก
        Task<IdentityResult> RegisterAsync(RegisterDto registerDto);

        // Login (ขอ token)
        Task<TokenResponse> LoginAsync(LoginDto loginDto);

        // Logout (สำหรับ session/cookie)
        Task LogoutAsync();

        // Refresh token
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenDto dto);

        // Revoke token (refresh token)
        Task<TokenRevocationResponse> RevokeTokenAsync(RevokeTokenDto dto);

        // Forgot password (ส่งลิงก์ reset)
        Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);

        // Change password
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto dto);

        // Reset password (ใช้ token ที่ได้จาก forgot)
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto);

        // ดึงข้อมูล user ปัจจุบัน
        Task<UserInfoDto> GetMeAsync(ClaimsPrincipal user);
        Task<TokenResponse> ExternalLoginAsync(ExternalLoginDto dto);
    }


}