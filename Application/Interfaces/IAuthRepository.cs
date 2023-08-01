using Domain.Master;

namespace Application.Interfaces
{
    public interface IAuthRepository 
    {
        Task<User> Login(string username, string password);
    }
}
