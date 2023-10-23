
namespace Application.Dto.Users
{
    /// <summary>Thông tin xóa người dùng</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class DeletedUserInput
    {
        /// <summary>Danh sách Id cần xóa</summary>
        public List<Guid> ListId { get; set; }

        /// <summary>Id đang đăng nhập</summary>
        public Guid? CurrentUserId { get; set; }

        /// <summary>Tên người dùng đang đăng nhập</summary>
        public string? CurrentEmpName { get; set; }
    }
}
