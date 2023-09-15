using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Common;
using VehicleInformation.DbContext;
using VehicleInformation.Interfaces.IRepository;
using VehicleInformation.Models;

namespace VehicleInformation.Repository
{
    public class ActivitySummariesRepository : GenericRepository<Report_ActivitySummaries>, IActivitySummariesRepository
    {
        public ActivitySummariesRepository(DapperContext _dbContext, ILogger<ActivitySummariesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
