

using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Users
{
    public class CreateOrEditUserDto
    {
        public Guid? UserId { get; set; }

        // Tên đăng nhập
        [StringLength(50)]
        public string? UserName { get; set; }

        // Mật khẩu
        [StringLength(100)]
        public string? Password { get; set; }
        [StringLength(500)]

        public string? Email { get; set; }

        // Tên người dùng
        [StringLength(200)]
        public string? EmpName { get; set; }

        // Ngày sinh
        public DateTime? BirthDay { get; set; }

        // Người tạo
        public Guid? CreatorUserId { get; set; }

        // Giới tính
        public byte? Gender { get; set; }

        // Số điện thoại
        [StringLength(10)]
        public string? PhoneNumber { get; set; }

        // Id người đang đăng nhập
        public Guid? CurrentUserId { get; set; }

        // Quyền
        public byte? UserType { get; set; }
    }
}
