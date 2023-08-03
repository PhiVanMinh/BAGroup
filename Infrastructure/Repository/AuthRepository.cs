using Application.Interfaces;
using Domain.Master;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDBContext _context;

        public AuthRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        #region -- Đăng nhập
        // Tìm kiếm thông tin user theo thông tin đầu vào , xác minh thông tin đăng nhập
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        // Xác minh mật khẩu
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
