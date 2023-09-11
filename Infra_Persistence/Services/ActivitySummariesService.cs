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
    /// <summary>Lây thông tin tổng hợp</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class ActivitySummariesService : IActivitySummariesService
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<ActivitySummariesService> _logger;

        public ActivitySummariesService(
            IUnitOfWork unitOfWork,
            ILogger<ActivitySummariesService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>Lấy thông tin tổng hợp theo đơn vị vẫn tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin tổng hợp</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<IEnumerable<ActivitySummaries>> GetAllByCompany(int input)
        {
            var result = new List<ActivitySummaries>();
            try
            {
                var summaries = await _unitOfWork.ActivitySummariesRepository.GetAllByCompany(input);
                result = summaries.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetActivitySummaries_{ex.Message}_{input}");
            }
            return result;
        }
    }
}
