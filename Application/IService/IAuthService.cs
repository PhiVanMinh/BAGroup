using Application.Dto.Users;
using Domain.Master;


namespace Application.IService
{
    public interface IAuthService
    {
        public Task<User> Login(UserForLoginDto userLogin);
    }
}
