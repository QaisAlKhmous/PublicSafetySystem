using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class LoginResultDTO
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public int Type { get; set; }
        public bool IsPassword { get; set; }
    }
}
