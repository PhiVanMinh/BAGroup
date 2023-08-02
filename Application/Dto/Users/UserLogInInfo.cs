using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Users
{
    public class UserLoginInfo
    {
        public int Id { get; set; }

        public string? EmpName { get; set; }

        public string? Token { get; set; }

        public byte? Role { get; set; }
    }
}
