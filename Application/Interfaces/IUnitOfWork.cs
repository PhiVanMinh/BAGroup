

namespace Application.Interfaces
{
    public interface IUnitOfWork 
    {
        IUserRepository UserRepository { get; }
        IAuthRepository AuthRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        void Save();
        void RollBack();
        Task SaveAsync();
        Task RollBackAsync();
    }
}
