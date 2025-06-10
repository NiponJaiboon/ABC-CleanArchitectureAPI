using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace Application.Services
{
    public class UserAdminService : IUserAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // ...service อื่นๆที่ไม่ใช่ IUserAdminService

        public UserAdminService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        // ...service อื่นๆ
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            // ...
        }

        public async Task<bool> EditUserAsync(string userId, EditUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Example: update user properties from dto
            user.Email = dto.Email;
            user.UserName = dto.UserName;
            // Add more property mappings as needed

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<IEnumerable<UserInfoDto>> GetAllUsersAsync(
            string search = null,
            string role = null
        )
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(u =>
                    u.UserName.Contains(search) || u.Email.Contains(search)
                );
            }

            if (!string.IsNullOrEmpty(role))
            {
                var roleUsers = await _userManager.GetUsersInRoleAsync(role);
                usersQuery = usersQuery.Where(u => roleUsers.Select(r => r.Id).Contains(u.Id));
            }

            var users = await usersQuery.ToListAsync();

            // Map ApplicationUser to UserInfoDto
            var userDtos = users.Select(u => new UserInfoDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                // Map other properties as needed
            });

            return userDtos;
        }

        public async Task<UserInfoDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            return new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                // Map other properties as needed
            };
        }

        public async Task<bool> SuspendUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Example: set a flag to indicate suspension (you must add this property to ApplicationUser)
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
