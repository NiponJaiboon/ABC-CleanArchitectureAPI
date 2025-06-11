using Shared.DTOs;

namespace Core.Interfaces
{
    public interface IUserAdminService
    {
        Task<IEnumerable<UserInfoDto>> GetAllUsersAsync(string? search = null, string? role = null);

        Task<UserInfoDto> GetUserByIdAsync(string userId);

        Task<bool> EditUserAsync(string userId, EditUserDto dto);

        Task<bool> SuspendUserAsync(string userId);
    }
}
