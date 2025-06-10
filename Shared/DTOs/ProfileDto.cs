namespace Shared.DTOs
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Role { get; set; } // เช่น "User", "Admin" เป็นต้น
        public string Bio { get; set; } // ข้อมูลเพิ่มเติมเกี่ยวกับผู้ใช้
        public string Location { get; set; } // ที่อยู่หรือเมืองของผู้ใช้
        public string Website { get; set; } // เว็บไซต์ส่วนตัวหรือโซเชียลมีเดีย
        public string Language { get; set; } // ภาษาโปรดของผู้ใช้
        public string TimeZone { get; set; } // โซนเวลาของผู้ใช้
        public string Gender { get; set; } // เพศของผู้ใช้
        public DateTime? DateOfBirth { get; set; } // วันเกิดของผู้ใช้
        public string[] Interests { get; set; } // ความสนใจของผู้ใช้
        public string[] Skills { get; set; } // ทักษะหรือความสามารถพิเศษ
        public string[] SocialLinks { get; set; } // ลิงก์โซเชียลมีเดีย เช่น Facebook, Twitter, LinkedIn
        public string[] Notifications { get; set; } // การแจ้งเตือนที่ผู้ใช้ตั้งค่าไว้
        public string[] Preferences { get; set; } // การตั้งค่าหรือความชอบส่วนตัว
    }
}
