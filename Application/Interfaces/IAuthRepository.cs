using Domain.Master;

namespace Application.Interfaces
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public interface IAuthRepository 
    {
        Task<User> Login(string username, string password);
    }
}
