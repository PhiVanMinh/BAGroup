using Application.Dto.Users;
using Application.Interfaces;
using Application.IService;
using AutoMapper;
using Domain.Master;

namespace Infrastructures.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<User> Login(UserForLoginDto userLogin)
        {
            var userFromRepo = await _unitOfWork.AuthRepository.Login(userLogin.UserName.ToLower(), userLogin.Password);
            return userFromRepo;


        }
    }
}
