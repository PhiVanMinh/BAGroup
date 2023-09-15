using Microsoft.Extensions.Logging;
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
    public class SpeedOversService: ISpeedOversService
    {
        private readonly ISpeedOversRepository _speedOversRepository;
        private readonly ILogger<SpeedOversService> _logger;
        public SpeedOversService(
            ISpeedOversRepository speedOversRepository,
            ILogger<SpeedOversService> logger
            )
        {
            _speedOversRepository = speedOversRepository;
            _logger = logger;
        }

        /// <summary>Thông tin vi phạm tốc độ theo ngày</summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Các thông tin vi phạm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public Task<List<BGT_SpeedOvers>> GetAllSpeedOversByDate(DateTime fromDate, DateTime toDate)
        {
            var result = new List<BGT_SpeedOvers>();
            try
            {
                var speedOvers = _speedOversRepository.GetAllByDate("StartTime", fromDate, toDate, false);
                result = speedOvers.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetSpeedOvers_{ex.Message}_{fromDate}_{toDate}");
            }
            return Task.FromResult(result);
        }
    }
}
