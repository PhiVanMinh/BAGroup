using Microsoft.Extensions.Logging;
using Services.Common.Core.Models;
using VehicleInformation.Common;
using VehicleInformation.DbContext;
using VehicleInformation.Interfaces.IRepository;

namespace VehicleInformation.Repository
{
    public class TransportTypesRepository : GenericRepository<BGT_TranportTypes>, ITransportTypesRepository
    {
        public TransportTypesRepository(DapperContext _dbContext, ILogger<TransportTypesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
