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

        // Tên người dùng
        public string? EmpName { get; set; }

        public string? Token { get; set; }

        // Quyền
        public byte? Role { get; set; }
    }
}
