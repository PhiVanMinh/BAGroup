using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReportSpeedOver.API.Common.Interfaces.IHelper;
using Services.Common.Core.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Interfaces.IRepository;
using VehicleInformation.Interfaces.IService;
using VehicleInformation.Repository;

namespace VehicleInformation.Services
{
    /// <summary>Thông tin tổng hợp</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/18/2023   created
    /// </Modified>
    public class ActivitySummariesService: IActivitySummariesService
    {
        private readonly IActivitySummariesRepository _activitySummariesRepository;
        private readonly ILogger<ActivitySummariesService> _logger;
        private readonly IRedisCacheHelper _cacheHelper;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _cache;

        public ActivitySummariesService(
            IActivitySummariesRepository activitySummariesRepository,
            ILogger<ActivitySummariesService> logger,
            IConfiguration configuration,
            IRedisCacheHelper cacheHelper
            )
        {
            _activitySummariesRepository = activitySummariesRepository;
            _logger = logger;
            _cacheHelper = cacheHelper;
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Lấy thông tin tổng hợp theo đơn vị vẫn tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin tổng hợp</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<List<Report_ActivitySummaries>> GetAllByCompany(int input)
        {
            var result = new List<Report_ActivitySummaries>();
            try
            {
                var cacheKey = $"ActivitySummariesService_GetAllByCompany_Report_ActivitySummaries_{input}";
                result = await _cacheHelper.GetDataFromCache<Report_ActivitySummaries>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var summaries = _activitySummariesRepository.GetAllByColumnId("FK_CompanyID", input, false);
                    result = summaries.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetActivitySummaries_{ex.Message}_{input}");
            }
            return result;
        }
    }
}
