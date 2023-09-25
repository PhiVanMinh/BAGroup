using Microsoft.Extensions.Logging;
using Services.Common.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Common;
using VehicleInformation.DbContext;
using VehicleInformation.Interfaces.IRepository;

namespace VehicleInformation.Repository
{
    public class SpeedOversRepository : GenericRepository<BGT_SpeedOvers>, ISpeedOversRepository
    {
        public SpeedOversRepository(DapperContext _dbContext, ILogger<SpeedOversRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
