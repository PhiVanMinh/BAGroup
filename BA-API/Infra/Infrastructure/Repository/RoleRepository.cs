using Application.Interfaces;
using Infrastructure.Common;
using Infrastructure.Persistence;
using Services.Common.Core.Entity;

namespace Persistence.Repository
{
    /// <summary>Repository quyền</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDBContext _dbContext) : base(_dbContext)
        {
        }
    }
}
