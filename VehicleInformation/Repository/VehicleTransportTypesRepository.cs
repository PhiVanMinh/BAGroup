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
    public class VehicleTransportTypesRepository : GenericRepository<BGT_VehicleTransportTypes>, IVehicleTransportTypesRepository
    {
        public VehicleTransportTypesRepository(DapperContext _dbContext, ILogger<VehicleTransportTypesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
