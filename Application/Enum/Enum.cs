using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Enum
{
    public enum FilterType : byte
    {
        UserName = 1, // Tên đăng nhập
        FullName = 2, // Tên người dùng
        Email = 3, // Email
        PhoneNumber = 4 // Số điện thoại
    }

    public enum Status : byte
    {
        No = 0,
        Yes = 1
    }
}
