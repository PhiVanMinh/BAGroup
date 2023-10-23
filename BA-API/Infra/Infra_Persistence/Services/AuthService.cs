using Application.Dto.Login;
using Application.Dto.Users;
using Application.Interfaces;
using Application.IService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infra_Persistence.Services
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/10/2023   created
    /// </Modified>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AuthService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>Tìm kiếm thông tin user theo thông tin đầu vào , xác minh thông tin đăng nhập</summary>
        /// <param name="userLogin">Thông tin đăng nhập</param>
        /// <returns>Thông tin người dùng, các quyền</returns>
        public async Task<ResponLoginDto> Login(UserForLoginDto userLogin)
        {
            var result = new ResponLoginDto();
            try
            {
                var userFromRepo = await _unitOfWork.AuthRepository.Login((userLogin.UserName ?? "").ToLower(), (userLogin.Password ?? ""));

                if (userFromRepo != null && userFromRepo.UserId != Guid.Empty)
                {
                    var roles = await (from role in _unitOfWork.RoleRepository.GetAll().Where(e => e.Status == 1)
                                       join ur in _unitOfWork.UserRoleRepository.GetAll().AsNoTracking() on role.Id equals ur.RoleId
                                       where ur.UserId == userFromRepo.UserId.ToString()
                                       select role.RoleName).ToListAsync();
                    result.Roles = roles;
                    result.User = userFromRepo;
                }
            } catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return result;
        }
    }
}
