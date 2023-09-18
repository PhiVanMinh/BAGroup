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
    /// <summary>Thông tin tổng hợp</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/18/2023   created
    /// </Modified>
    public class ActivitySummariesService: IActivitySummariesService
    {
        private readonly IActivitySummariesRepository _activitySummariesRepository;
        private readonly ILogger<ActivitySummariesService> _logger;

        public ActivitySummariesService(
            IActivitySummariesRepository activitySummariesRepository,
            ILogger<ActivitySummariesService> logger
            )
        {
            _activitySummariesRepository = activitySummariesRepository;
            _logger = logger;
        }

        /// <summary>Lấy thông tin tổng hợp theo đơn vị vẫn tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin tổng hợp</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public Task<List<Report_ActivitySummaries>> GetAllByCompany(int input)
        {
            var result = new List<Report_ActivitySummaries>();
            try
            {
                var summaries = _activitySummariesRepository.GetAllByColumnId("FK_CompanyID", input, false);
                result = summaries.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetActivitySummaries_{ex.Message}_{input}");
            }
            return Task.FromResult(result);
        }
    }
}
