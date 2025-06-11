using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
        public string Role { get; set; } // เพิ่ม field นี้ถ้าต้องการให้เลือก role ตอนสมัคร
    }
}
