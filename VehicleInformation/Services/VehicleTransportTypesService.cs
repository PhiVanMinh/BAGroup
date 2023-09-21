using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReportSpeedOver.API.Common.Interfaces.IHelper;
using StackExchange.Redis;
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
        private readonly IConfiguration _configuration;
        private readonly IVehicleTransportTypesRepository _vehicleTransportTypesRepository;
        private readonly ILogger<VehicleTransportTypesService> _logger;
        private readonly IRedisCacheHelper _cacheHelper;
        private readonly IDatabase _cache;

        public VehicleTransportTypesService(
            IVehicleTransportTypesRepository vehicleTransportTypesRepository,
            ILogger<VehicleTransportTypesService> logger,
            IConfiguration configuration,
            IRedisCacheHelper cacheHelper
            )
        {
            _vehicleTransportTypesRepository = vehicleTransportTypesRepository;
            _logger = logger;
            _cacheHelper = cacheHelper;
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Thông tin loại hình vận tải của phương tiện</summary>
        /// <returns>Các thông tin loại hình vận tải của phương tiện</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<List<BGT_VehicleTransportTypes>> GetAll()
        {
            var result = new List<BGT_VehicleTransportTypes>();
            try
            {
                var cacheKey = $"VehicleTransportTypesService_GetAll_BGT_VehicleTransportTypes";
                result = await _cacheHelper.GetDataFromCache<BGT_VehicleTransportTypes>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var respon = _vehicleTransportTypesRepository.GetAll();
                    result = respon.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicleTransportTypes_{ex.Message}");
            }
            return result;
        }
    }
}
