
namespace Application.Dto.Users
{
    public class DeletedUserInput
    {
        // Danh sách Id các người dùng cần xóa
        public List<int> ListId { get; set; }

        // Id người dùng đang đăng nhập
        public int CurrentUserId { get; set; }

        // Tên người đang đăng nhập
        public string? CurrentEmpName { get; set; }
    }
}
