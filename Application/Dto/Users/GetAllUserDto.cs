
namespace Application.Dto.Users
{
    /// <summary>Danh sách người dùng</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class GetAllUserDto
    {
        /// <summary>Id người dùng</summary>
        public Guid UserId { get; set; }

        /// <summary>Tên đăng nhập</summary>
        public string? UserName { get; set; }

        /// <summary>Email</summary>
        public string? Email { get; set; }

        /// <summary>Tên người dùng</summary>
        public string? EmpName { get; set; }

        /// <summary>Ngày sinh</summary>
        public DateTime? BirthDay { get; set; }

        /// <summary>Giới tính</summary>
        public byte? Gender { get; set; }

        /// <summary>Số điện thoại</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Loại người dùng</summary>
        public byte? UserType { get; set; }
    }
}
