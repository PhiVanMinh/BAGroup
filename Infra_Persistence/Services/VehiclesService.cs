using Application.Interfaces;
using Application.IService;
using Application.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;


namespace Infra_Persistence.Services
{
    /// <summary>Thông tin xe</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class VehiclesService : IVehiclesService
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<VehiclesService> _logger;

        public VehiclesService(
            IUnitOfWork unitOfWork,
            ILogger<VehiclesService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>Lấy thông tin xe theo đơn vị vận tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<IEnumerable<Vehicles>> GetAllByCompany(int input)
        {
            var result = new List<Vehicles>();
            try
            {
                var vehicles = await _unitOfWork.VehiclesRepository.GetAllByCompany(input);
                result = vehicles.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicles_{ex.Message}_{input}");
            }
            return result;
        }
    }
}
