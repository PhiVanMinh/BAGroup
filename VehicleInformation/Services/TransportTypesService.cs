using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VehicleInformation.Interfaces.IRepository;
using VehicleInformation.Interfaces.IService;
using VehicleInformation.Models;
using VehicleInformation.Repository;

namespace VehicleInformation.Services
{
    /// <summary>Thông tin loại hình vận chuyển</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/18/2023   created
    /// </Modified>
    public class TransportTypesService: ITransportTypesService
    {
        private readonly ITransportTypesRepository _transportTypeRepo;
        private readonly ILogger<TransportTypesService> _logger;
        public TransportTypesService(
            ITransportTypesRepository transportTypeRepo,
            ILogger<TransportTypesService> logger
            )
        {
            _transportTypeRepo = transportTypeRepo;
            _logger = logger;
        }

        /// <summary>Lấy các thông tin loại hình vận tải</summary>
        /// <returns> Các thông tin loại hình vận tải</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public Task<List<BGT_TranportTypes>> GetAll()
        {
            var result = new List<BGT_TranportTypes>();
            try
            {
                var types = _transportTypeRepo.GetAll();
                result = types.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetTranportTypes_{ex.Message}");
            }
            return Task.FromResult(result);
        }
    }
}
