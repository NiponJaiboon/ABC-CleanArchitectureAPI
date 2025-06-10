namespace Shared.DTOs
{
    public class EditUserDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; } // ใช้สำหรับการเปลี่ยนชื่อผู้ใช้
    }
}
