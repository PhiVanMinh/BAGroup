using Application.Interfaces;
using Domain.Master;
using Infrastructure.Common;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repository
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDBContext _dbContext) : base(_dbContext)
        {
        }
    }
}
