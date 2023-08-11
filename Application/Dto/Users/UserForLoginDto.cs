
namespace Application.Dto.Users
{
    /// <summary>Thông tin đăng nhập</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class UserForLoginDto
    {
        /// <summary>Tên người dùng</summary>
        public string? UserName { get; set; }

        /// <summary>Mật khẩu</summary>
        public string? Password { get; set; }
    }
}
