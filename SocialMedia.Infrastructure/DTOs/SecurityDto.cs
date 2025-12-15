using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.Enum;

namespace Movies.Infrastructure.DTOs
{
    public class SecurityDto
    {
        public int UserId { get; set; }
        public string Login { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public RoleType? Role { get; set; }
    }
}










