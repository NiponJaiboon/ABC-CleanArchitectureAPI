using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class ExternalLoginDto
    {
        public string Provider { get; set; } // "Google", "Facebook", "Microsoft"
        public string IdToken { get; set; }  // สำหรับ Google/Microsoft (JWT)
        public string AccessToken { get; set; } // สำหรับ Facebook
        public string UserId { get; set; }
    }
}