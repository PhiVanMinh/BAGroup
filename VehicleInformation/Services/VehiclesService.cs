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
    /// <summary>Thông tin xe</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/18/2023   created
    /// </Modified>
    public class VehiclesService: IVehiclesService
    {
        private readonly IVehiclesRepository _vhcRepo;
        private readonly ILogger<VehiclesService> _logger;
        private readonly IRedisCacheHelper _cacheHelper;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _cache;

        public VehiclesService(
            IVehiclesRepository vhcRepo,
            ILogger<VehiclesService> logger,
            IRedisCacheHelper cacheHelper
            )
        {
            _vhcRepo = vhcRepo;
            _logger = logger;
            _cacheHelper = cacheHelper;
            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Lấy thông tin xe theo đơn vị vận tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<List<Vehicle_Vehicles>> GetAllByCompany(int input)
        {
            var result = new List<Vehicle_Vehicles>();
            try
            {
                var cacheKey = $"VehiclesService_GetAllByCompany_Vehicle_Vehicles";
                result = await _cacheHelper.GetDataFromCache<Vehicle_Vehicles>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var vehicles = _vhcRepo.GetAllByColumnId("FK_CompanyID", input, true);
                    result = vehicles.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicles_{ex.Message}_{input}");
            }
            return result;
        }
    }
}
