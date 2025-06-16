using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize(Policy = "GeneralUser")]
        [HttpGet()]
        public async Task<IActionResult> GetProfile()
        {
            // Debug logs (for development only)
            Console.WriteLine("User.Identity.IsAuthenticated: " + User.Identity.IsAuthenticated);
            Console.WriteLine("Authorization header: " + Request.Headers["Authorization"]);
            // แสดงข้อมูล Role และ Policy ของผู้ใช้ (ถ้ามี)
            var roles = User
                .Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            Console.WriteLine("User Roles: " + string.Join(", ", roles));

            // ตัวอย่างการดึง Policy (จริงๆ แล้ว Policy จะถูกใช้ใน Authorize Attribute)
            // แต่สามารถตรวจสอบ Claims อื่นๆ ได้เช่นกัน
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            // ตรวจสอบว่าเวลาของเซิร์ฟเวอร์ตรงกันหรือไม่ (สำหรับการสาธิต, แสดง log เวลาเซิร์ฟเวอร์ปัจจุบัน)
            Console.WriteLine("Server Time (UTC): " + DateTime.UtcNow.ToString("o"));

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _authService.GetProfileAsync(userId);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut()]
        public async Task<IActionResult> EditProfile(EditProfileDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _authService.EditProfileAsync(userId, dto);
            if (!result)
                return BadRequest("Failed to update profile");
            return NoContent();
        }
    }
}
