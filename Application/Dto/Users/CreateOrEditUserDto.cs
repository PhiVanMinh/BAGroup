

using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Users
{
    /// <summary>Thông tin người dùng cần thêm, sửa</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class CreateOrEditUserDto
    {
        public Guid? UserId { get; set; }

        /// <summary>Tên đăng nhập</summary>
        [StringLength(50)]
        public string? UserName { get; set; }

        /// <summary>Mật khẩu</summary>
        [StringLength(100)]
        public string? Password { get; set; }

        /// <summary>Email</summary>
        [StringLength(500)]
        public string? Email { get; set; }

        /// <summary>Tên người dùng</summary>
        [StringLength(200)]
        public string? EmpName { get; set; }

        /// <summary>Ngày sinh</summary>
        public DateTime? BirthDay { get; set; }

        /// <summary>Người tạo</summary>
        public Guid? CreatorUserId { get; set; }

        /// <summary>Giới tính</summary>
        public byte? Gender { get; set; }

        /// <summary>Số điện thoại</summary>
        [StringLength(10)]
        public string? PhoneNumber { get; set; }

        /// <summary>Id người đăng nhập</summary>
        public Guid? CurrentUserId { get; set; }

        /// <summary>Loại người dùng</summary>
        public byte? UserType { get; set; }
    }
}
