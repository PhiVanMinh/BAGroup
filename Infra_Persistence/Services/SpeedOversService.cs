using Application.Interfaces;
using Application.IService;
using Application.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infra_Persistence.Services
{
    /// <summary>Thông tin vi phạm tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class SpeedOversService: ISpeedOversService
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeedOversService> _logger;

        public SpeedOversService(
            IUnitOfWork unitOfWork,
            ILogger<SpeedOversService> logger
            )
        {
            _unitOfWork = unitOfWork;
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
        public async Task<IEnumerable<SpeedOvers>> GetAllSpeedOversByDate(DateTime? fromDate, DateTime? toDate)
        { 
            var result  = new List<SpeedOvers>();
            try
            {
               var speedOvers = await _unitOfWork.SpeedOversRepository.GetAllByDate(fromDate, toDate);
                result = speedOvers.ToList();
            } catch ( Exception ex )
            {
                _logger.LogInformation($"GetSpeedOvers_{ex.Message}_{fromDate}_{toDate}");
            }
            return result;
        }
    }
}
