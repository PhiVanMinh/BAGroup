using ReportDataGrpcService.Common;
using ReportDataGrpcService.DBContext;
using ReportDataGrpcService.Interfaces.IRepository;
using Services.Common.Core.Models;

namespace ReportDataGrpcService.Repository
{
    public class TransportTypesRepository : Repository<BGT_TranportTypes>, ITransportTypesRepository
    {
        public TransportTypesRepository(DapperDbContext _dbContext, ILogger<TransportTypesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
