using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");
            return await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email address");
            }

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<TokenResponse> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var result = _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true)
                .Result;

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var authority = _configuration["IdentityServer:Authority"];
            using var httpClient = new HttpClient();
            var discoveryDocument = httpClient.GetDiscoveryDocumentAsync(authority).Result;

            return await httpClient.RequestPasswordTokenAsync(
                new PasswordTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "my-client",
                    ClientSecret = "secret",
                    UserName = loginDto.Username,
                    Password = loginDto.Password,
                }
            );
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var authority = _configuration["IdentityServer:Authority"];
            using var httpClient = new HttpClient();
            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(authority);

            return await httpClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "my-client",
                    ClientSecret = "secret",
                    RefreshToken = dto.RefreshToken,
                }
            );
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                var role = string.IsNullOrEmpty(registerDto.Role) ? "User" : registerDto.Role;
                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }

        public async Task<TokenRevocationResponse> RevokeTokenAsync(RevokeTokenDto dto)
        {
            var authority = _configuration["IdentityServer:Authority"];
            using var httpClient = new HttpClient();
            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(authority);

            return await httpClient.RevokeTokenAsync(
                new TokenRevocationRequest
                {
                    Address = discoveryDocument.RevocationEndpoint,
                    ClientId = "my-client",
                    ClientSecret = "secret",
                    Token = dto.Token,
                }
            );
        }

        public async Task<UserInfoDto> GetMeAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
                return null;

            return new UserInfoDto
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                // Add other properties as needed
            };
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            return result;
        }

        public async Task<TokenResponse> ExternalLoginAsync(ExternalLoginDto dto)
        {
            // 1. ตรวจสอบ token กับ provider (Google, Facebook, Microsoft)
            // 2. หา user ในระบบ ถ้าไม่มีให้สร้างใหม่
            // 3. ออก JWT/Token กลับไป

            string email = null;
            string providerUserId = null;

            if (dto.Provider == "Google")
            {
                // ตรวจสอบ Google IdToken
                var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(
                    dto.IdToken
                );
                email = payload.Email;
                providerUserId = payload.Subject;
            }
            else if (dto.Provider == "Facebook")
            {
                // ตรวจสอบ Facebook AccessToken (ใช้ Facebook Graph API)
                // ... (ต้องเขียนโค้ดเรียก Facebook API)
            }
            else if (dto.Provider == "Microsoft")
            {
                // ตรวจสอบ Microsoft IdToken (JWT)
                // ... (ต้อง decode JWT และ validate)
            }
            else
            {
                throw new Exception("Provider not supported");
            }

            // หา user ในระบบ
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email };
                await _userManager.CreateAsync(user);
                // อาจต้อง AddLoginAsync ด้วย
            }

            // ออก token (เหมือน login ปกติ)
            var authority = _configuration["IdentityServer:Authority"];
            using var httpClient = new HttpClient();
            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(authority);

            return await httpClient.RequestPasswordTokenAsync(
                new PasswordTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "my-client",
                    ClientSecret = "secret",
                    UserName = user.UserName,
                    // หรือ generate password เฉพาะสำหรับ external
                    // Scope = "api1 openid profile offline_access",
                }
            );
        }

        public async Task<ProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            // สามารถดึงข้อมูลเพิ่มเติมได้ตามต้องการ เช่น Role, Profile Picture, ฯลฯ
            var roles = await _userManager.GetRolesAsync(user);
            var phoneNumber = user.PhoneNumber ?? string.Empty;

            // แต่ในที่นี้จะดึงแค่ข้อมูลพื้นฐาน
            if (user == null)
                return null;
            if (string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("User not found or email is empty");
            if (string.IsNullOrEmpty(user.UserName))
                throw new UnauthorizedAccessException("User not found or username is empty");

            return new ProfileDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "User",
                PhoneNumber = phoneNumber,
            };
        }

        public async Task<bool> EditProfileAsync(string userId, EditProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(dto.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            return result.Succeeded;
        }
    }
}
