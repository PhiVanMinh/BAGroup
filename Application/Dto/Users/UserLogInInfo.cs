using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Users
{
    /// <summary>Thông tin người dùng sau đăng nhập</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class UserLoginInfo
    {
        /// <summary>Id người dùng</summary>
        public Guid UserId { get; set; }

        /// <summary>Tên người dùng</summary>
        public string? EmpName { get; set; }

        /// <summary>Mã phiên đăng nhập</summary>
        public string? Token { get; set; }

        /// <summary>Loại người dùng</summary>
        public byte? UserType { get; set; }

        /// <summary>Mã công ty</summary>
        public int? CompanyId { get; set; }
    }
}
