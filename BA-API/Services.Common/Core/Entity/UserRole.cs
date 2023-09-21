using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common.Core.Entity
{
    /// <summary>bảng quyền của người dùng</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class UserRole : BaseEntity<int>
    {
        /// <summary>Mã người dùng</summary>
        public string UserId { get; set; }

        /// <summary>Mã quyền</summary>
        public int RoleId { get; set; }
    }
}
