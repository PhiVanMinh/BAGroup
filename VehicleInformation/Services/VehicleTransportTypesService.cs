using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Interfaces.IRepository;
using VehicleInformation.Interfaces.IService;
using VehicleInformation.Models;

namespace VehicleInformation.Services
{
    /// <summary>Thông tin xe gắn với loại hình</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/18/2023   created
    /// </Modified>
    public class VehicleTransportTypesService: IVehicleTransportTypesService
    {
        private readonly IVehicleTransportTypesRepository _vehicleTransportTypesRepository;
        private readonly ILogger<VehicleTransportTypesService> _logger;

        public VehicleTransportTypesService(
            IVehicleTransportTypesRepository vehicleTransportTypesRepository,
            ILogger<VehicleTransportTypesService> logger
            )
        {
            _vehicleTransportTypesRepository = vehicleTransportTypesRepository;
            _logger = logger;
        }

        /// <summary>Thông tin loại hình vận tải của phương tiện</summary>
        /// <returns>Các thông tin loại hình vận tải của phương tiện</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public Task<List<BGT_VehicleTransportTypes>> GetAll()
        {
            var result = new List<BGT_VehicleTransportTypes>();
            try
            {
                var vhcTypes = _vehicleTransportTypesRepository.GetAll();
                result = vhcTypes.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicleTransportTypes_{ex.Message}");
            }
            return Task.FromResult(result);
        }
    }
}
