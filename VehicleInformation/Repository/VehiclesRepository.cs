using Dapper;
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
    /// <summary>Thông tin xe</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class VehiclesRepository : GenericRepository<Vehicle_Vehicles>, IVehiclesRepository
    {
        public VehiclesRepository(DapperContext _dbContext, ILogger<VehiclesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
