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
using VehicleInformation.Repository;

namespace VehicleInformation.Services
{
    /// <summary>Thông tin vi phạm tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/18/2023   created
    /// </Modified>
    public class SpeedOversService: ISpeedOversService
    {
        private readonly ISpeedOversRepository _speedOversRepository;
        private readonly ILogger<SpeedOversService> _logger;
        private readonly IRedisCacheHelper _cacheHelper;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _cache;

        public SpeedOversService(
            ISpeedOversRepository speedOversRepository,
            ILogger<SpeedOversService> logger,
            IRedisCacheHelper cacheHelper
            )
        {
            _speedOversRepository = speedOversRepository;
            _logger = logger;
            _cacheHelper = cacheHelper;
            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Thông tin vi phạm tốc độ theo ngày</summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Các thông tin vi phạm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<List<BGT_SpeedOvers>> GetAllSpeedOversByDate(DateTime fromDate, DateTime toDate)
        {
            var result = new List<BGT_SpeedOvers>();
            try
            {
                var cacheKey = $"SpeedOversService_GetAllSpeedOversByDate_BGT_SpeedOvers";
                result = await _cacheHelper.GetDataFromCache<BGT_SpeedOvers>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var speedOvers = _speedOversRepository.GetAllByDate("StartTime", fromDate, toDate, false);
                    result = speedOvers.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetSpeedOvers_{ex.Message}_{fromDate}_{toDate}");
            }
            return result;
        }
    }
}
