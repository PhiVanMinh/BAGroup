using Application.Interfaces;
using Domain.Master;
using Infrastructure.Persistence;
using Infrastructure.Common;

namespace Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDBContext _dbContext) : base(_dbContext)
        {
        }
    }

}
