using Application.Dto.Login;
using Application.Dto.Users;
using Domain.Master;


namespace Application.IService
{
    public interface IAuthService
    {
        public Task<ResponLoginDto> Login(UserForLoginDto userLogin);
    }
}
