

namespace Application.Interfaces
{
    public interface IUnitOfWork 
    {
        IUserRepository UserRepository { get; }
        IAuthRepository AuthRepository { get; }
        void Save();
        void RollBack();
        Task SaveAsync();
        Task RollBackAsync();
    }
}
