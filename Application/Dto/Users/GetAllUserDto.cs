
namespace Application.Dto.Users
{
     public class GetAllUserDto
    {
        public Guid UserId { get; set; }

        // Tên đăng nhập
        public string? UserName { get; set; }

        public string? Email { get; set; }

        // Tên người dùng
        public string? EmpName { get; set; }

        // Ngày sinh
        public DateTime? BirthDay { get; set; }

        // Giới tính
        public byte? Gender { get; set; }

        // Số điện thoại
        public string? PhoneNumber { get; set; }
        // Loại tài khoản
        public byte? UserType { get; set; }
    }
}
