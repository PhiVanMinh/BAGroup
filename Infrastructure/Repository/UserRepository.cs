using Application.Interfaces;
using Domain.Master;
using Infrastructure.Persistence;
using Infrastructure.Common;

namespace Infrastructure.Repository
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDBContext _dbContext) : base(_dbContext)
        {
        }
    }

}
