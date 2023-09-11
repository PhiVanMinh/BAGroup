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

namespace Infra_Persistence.Services
{
    /// <summary>Thông tin loại hình vận tải</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class TransportTypesService: ITransportTypesService
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<TransportTypesService> _logger;

        public TransportTypesService(
            IUnitOfWork unitOfWork,
            ILogger<TransportTypesService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>Lấy các thông tin loại hình vận tải</summary>
        /// <returns> Các thông tin loại hình vận tải</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<IEnumerable<TranportTypes>> GetAll()
        {
            var result = new List<TranportTypes>();
            try
            {
                var types = await _unitOfWork.TranportTypesRepository.GetAll();
                result = types.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetTranportTypes_{ex.Message}");
            }
            return result;
        }
    }
}
