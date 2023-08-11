using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Enum
{
    /// <summary>Loại tìm kiếm</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public enum FilterType : byte
    {
        UserName = 1, // Tên đăng nhập
        FullName = 2, // Tên người dùng
        Email = 3, // Email
        PhoneNumber = 4 // Số điện thoại
    }

    /// <summary>Trạng thái</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public enum Status : byte
    {
        No = 0,
        Yes = 1
    }
}
