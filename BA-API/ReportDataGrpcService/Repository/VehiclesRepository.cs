using ReportDataGrpcService.Common;
using ReportDataGrpcService.DBContext;
using ReportDataGrpcService.Interfaces.IRepository;
using Services.Common.Core.Models;

namespace ReportDataGrpcService.Repository
{
    /// <summary>Thông tin xe</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class VehiclesRepository : Repository<Vehicle_Vehicles>, IVehiclesRepository
    {
        public VehiclesRepository(DapperDbContext _dbContext, ILogger<VehiclesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
