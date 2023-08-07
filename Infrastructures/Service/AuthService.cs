using Application.Dto.Login;
using Application.Dto.Users;
using Application.Interfaces;
using Application.IService;
using AutoMapper;
using Domain.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region -- Đăng nhập
        // Tìm kiếm thông tin user theo thông tin đầu vào , xác minh thông tin đăng nhập
        public async Task<ResponLoginDto> Login(UserForLoginDto userLogin)
        {
            var result = new ResponLoginDto();
            var userFromRepo = await _unitOfWork.AuthRepository.Login(userLogin.UserName.ToLower(), userLogin.Password);

            if( userFromRepo != null && userFromRepo.UserId.ToString() != "")
            {
                var roles = await (from role in _unitOfWork.RoleRepository.GetAll().Where(e => e.Status == 1)
                                   join ur in _unitOfWork.UserRoleRepository.GetAll().AsNoTracking() on role.Id equals ur.RoleId
                                   where ur.UserId == userFromRepo.UserId.ToString()
                                   select role.RoleName).ToListAsync();
                result.Roles = roles;
                result.User = userFromRepo;
            }
            return result;
        }
        #endregion
    }
}
