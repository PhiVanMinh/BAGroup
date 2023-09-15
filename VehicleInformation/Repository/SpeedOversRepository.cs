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
    public class SpeedOversRepository : GenericRepository<BGT_SpeedOvers>, ISpeedOversRepository
    {
        public SpeedOversRepository(DapperContext _dbContext, ILogger<SpeedOversRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
