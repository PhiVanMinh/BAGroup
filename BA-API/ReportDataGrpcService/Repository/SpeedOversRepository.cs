using ReportDataGrpcService.Common;
using ReportDataGrpcService.DBContext;
using ReportDataGrpcService.Interfaces.IRepository;
using Services.Common.Core.Models;

namespace ReportDataGrpcService.Repository
{
    public class SpeedOversRepository : Repository<BGT_SpeedOvers>, ISpeedOversRepository
    {
        public SpeedOversRepository(DapperDbContext _dbContext, ILogger<SpeedOversRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
