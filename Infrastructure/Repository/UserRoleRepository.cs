using Application.Interfaces;
using Infrastructure.Common;
using Infrastructure.Persistence;
using Services.Common.Core.Entity;

namespace Persistence.Repository
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDBContext _dbContext) : base(_dbContext)
        {
        }
    }
}
