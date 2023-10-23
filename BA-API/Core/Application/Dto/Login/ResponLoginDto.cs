using Services.Common.Core.Entity;


namespace Application.Dto.Login
{
    /// <summary>Dữ liệu phản hồi sau khi đăng nhập</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class ResponLoginDto
    {
        /// <summary>Thông tin người dùng</summary>
        public User? User { get; set; }

        /// <summary>Thông tin các quyền của người dùng</summary>
        public List<string> Roles { get; set; }
    }
}
