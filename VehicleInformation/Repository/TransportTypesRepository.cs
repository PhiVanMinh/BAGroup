using Microsoft.Extensions.Logging;

using VehicleInformation.Common;
using VehicleInformation.DbContext;
using VehicleInformation.Interfaces.IRepository;
using VehicleInformation.Models;

namespace VehicleInformation.Repository
{
    public class TransportTypesRepository : GenericRepository<BGT_TranportTypes>, ITransportTypesRepository
    {
        public TransportTypesRepository(DapperContext _dbContext, ILogger<TransportTypesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
