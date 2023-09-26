using ReportDataGrpcService.Common;
using ReportDataGrpcService.DBContext;
using ReportDataGrpcService.Interfaces.IRepository;
using Services.Common.Core.Models;

namespace ReportDataGrpcService.Repository
{
    public class ActivitySummariesRepository : Repository<Report_ActivitySummaries>, IActivitySummariesRepository
    {
        public ActivitySummariesRepository(DapperDbContext _dbContext, ILogger<ActivitySummariesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
