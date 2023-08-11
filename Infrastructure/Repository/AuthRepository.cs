using Application.Interfaces;
using Domain.Master;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    /// <summary>Repository đăng nhập</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDBContext _context;

        public AuthRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        #region -- Đăng nhập
        /// <summary>Tìm kiếm thông tin user theo thông tin đầu vào , xác minh thông tin đăng nhập</summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Thông tin người dùng</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/11/2023   created
        /// </Modified>
        public async Task<User> Login(string username, string password)
        {
            var user = new User();
            var userLogin = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (userLogin == null)
                return user;

            if (!VerifyPasswordHash(password, userLogin.PasswordHash, userLogin.PasswordSalt))
                return user;
            user = userLogin;
            return user;
        }

        /// <summary>Xác minh mật khẩu</summary>
        /// <param name="password">mật khẩu</param>
        /// <param name="passwordHash">Mật khẩu đã mã hóa băm</param>
        /// <param name="passwordSalt">Mật khẩu mã hóa trộn</param>
        /// <returns>
        ///   <para>bool</para>
        /// </returns>
        private bool VerifyPasswordHash(string password, string passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                if (Convert.ToBase64String(computedHash) != passwordHash) return false;
            }
            return true;
        }
        #endregion
    }
}
