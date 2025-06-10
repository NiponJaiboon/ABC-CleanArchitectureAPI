using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class RevokeTokenDto
    {
        public string Token { get; set; }
        public string TokenType { get; set; } // "refresh_token" หรือ "access_token"
        public string Username { get; set; } // Add this line if you want to pass username
    }
}
