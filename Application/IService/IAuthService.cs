using Application.Dto.Login;
using Application.Dto.Users;
using Domain.Master;


namespace Application.IService
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public interface IAuthService
    {
        public Task<ResponLoginDto> Login(UserForLoginDto userLogin);
    }
}
