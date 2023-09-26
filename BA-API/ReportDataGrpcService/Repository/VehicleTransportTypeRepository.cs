using ReportDataGrpcService.Common;
using ReportDataGrpcService.DBContext;
using ReportDataGrpcService.Interfaces.IRepository;
using Services.Common.Core.Models;

namespace ReportDataGrpcService.Repository
{
    public class VehicleTransportTypeRepository
    {
    }

    public class VehicleTransportTypesRepository : Repository<BGT_VehicleTransportTypes>, IVehicleTransportTypesRepository
    {
        public VehicleTransportTypesRepository(DapperDbContext _dbContext, ILogger<VehicleTransportTypesRepository> logger) : base(_dbContext, logger)
        {
        }
    }
}
