using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserAdminController : ControllerBase
    {
        private readonly IUserAdminService _userAdminService;

        public UserAdminController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(string search = null, string role = null)
        {
            var users = await _userAdminService.GetAllUsersAsync(search, role);
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userAdminService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPut("users/{userId}")]
        public async Task<IActionResult> EditUser(string userId, [FromBody] EditUserDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid user data");

            var result = await _userAdminService.EditUserAsync(userId, dto);
            if (!result)
                return BadRequest("Failed to update user");
            return NoContent();
        }

        [HttpPost("users/{userId}/suspend")]
        public async Task<IActionResult> SuspendUser(string userId)
        {
            var result = await _userAdminService.SuspendUserAsync(userId);
            if (!result)
                return BadRequest("Failed to suspend user");
            return NoContent();
        }
    }
}
