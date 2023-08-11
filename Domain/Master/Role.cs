using Domain.Common;

namespace Domain.Master
{
    /// <summary>Bảng thông tin quyền</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class Role : BaseEntity<int>
    {
        /// <summary>Tên quyền</summary>
        public string? RoleName { get; set; }

        /// <summary>Trạng thái quyền</summary>
        public byte? Status { get; set; }
    }
}
