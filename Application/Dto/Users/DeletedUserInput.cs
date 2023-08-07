
namespace Application.Dto.Users
{
    public class DeletedUserInput
    {
        // Danh sách Id các người dùng cần xóa
        public List<Guid> ListId { get; set; }

        // Id người dùng đang đăng nhập
        public Guid? CurrentUserId { get; set; }

        // Tên người đang đăng nhập
        public string? CurrentEmpName { get; set; }
    }
}
