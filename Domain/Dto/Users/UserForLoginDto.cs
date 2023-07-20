using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.Users
{
    public class UserForLoginDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
