using Application.Interfaces;
using Application.IService;
using Application.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infra_Persistence.Services
{
    /// <summary>Thông tin loại hình vận tải của phương tiện</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class VehicleTransportTypesService : IVehicleTransportTypesService
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<VehicleTransportTypesService> _logger;

        public VehicleTransportTypesService(
            IUnitOfWork unitOfWork,
            ILogger<VehicleTransportTypesService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>Thông tin loại hình vận tải của phương tiện</summary>
        /// <returns>Các thông tin loại hình vận tải của phương tiện</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<IEnumerable<VehicleTransportTypes>> GetAll()
        {
            var result = new List<VehicleTransportTypes>();
            try
            {
                var vhcTypes = await _unitOfWork.VehicleTransportTypesRepository.GetAll();
                result = vhcTypes.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicleTransportTypes_{ex.Message}");
            }
            return result;
        }
    }
}
