

namespace Application.Dto.Users
{
    public class CreateOrEditUserDto
    {
        public Guid? UserId { get; set; }

        // Tên đăng nhập
        public string? UserName { get; set; }

        // Mật khẩu
        public string? Password { get; set; }

        public string? Email { get; set; }

        // Tên người dùng
        public string? EmpName { get; set; }

        // Ngày sinh
        public DateTime? BirthDay { get; set; }

        // Người tạo
        public Guid? CreatorUserId { get; set; }

        // Giới tính
        public byte? Gender { get; set; }

        // Số điện thoại
        public string? PhoneNumber { get; set; }

        // Id người đang đăng nhập
        public Guid? CurrentUserId { get; set; }

        // Quyền
        public byte? Role { get; set; }
    }
}
