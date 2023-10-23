using Domain.Common;

namespace Domain.Master
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
