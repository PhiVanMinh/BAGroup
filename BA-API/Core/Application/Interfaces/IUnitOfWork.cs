

namespace Application.Interfaces
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public interface IUnitOfWork 
    {
        IUserRepository UserRepository { get; }
        IAuthRepository AuthRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        ISpeedViolationRepository SpeedViolationRepository { get; }
        void Save();
        void RollBack();
        Task SaveAsync();
        Task RollBackAsync();
    }
}
