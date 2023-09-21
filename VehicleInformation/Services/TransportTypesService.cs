using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReportSpeedOver.API.Common.Interfaces.IHelper;
using StackExchange.Redis;
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
        private readonly IRedisCacheHelper _cacheHelper;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _cache;

        public TransportTypesService(
            ITransportTypesRepository transportTypeRepo,
            ILogger<TransportTypesService> logger,
            IRedisCacheHelper cacheHelper
            )
        {
            _transportTypeRepo = transportTypeRepo;
            _logger = logger;
            _cacheHelper = cacheHelper;
            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Lấy các thông tin loại hình vận tải</summary>
        /// <returns> Các thông tin loại hình vận tải</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<List<BGT_TranportTypes>> GetAll()
        {
            var result = new List<BGT_TranportTypes>();
            try
            {
                var cacheKey = $"TransportTypesService_GetAll_BGT_TranportTypes";
                result = await _cacheHelper.GetDataFromCache<BGT_TranportTypes>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var types = _transportTypeRepo.GetAll();
                    result = types.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetTranportTypes_{ex.Message}");
            }
            return result;
        }
    }
}
