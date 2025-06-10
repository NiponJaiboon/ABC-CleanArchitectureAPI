namespace Shared.DTOs
{
    public class EditProfileDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } // เช่น "User", "Admin" เป็นต้น
    }
}
