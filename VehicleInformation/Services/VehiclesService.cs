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
    public class VehiclesService: IVehiclesService
    {
        private readonly IVehiclesRepository _vhcRepo;
        private readonly ILogger<VehiclesService> _logger;

        public VehiclesService(
            IVehiclesRepository vhcRepo,
            ILogger<VehiclesService> logger
            )
        {
            _vhcRepo = vhcRepo;
            _logger = logger;
        }

        /// <summary>Lấy thông tin xe theo đơn vị vận tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public Task<List<Vehicle_Vehicles>> GetAllByCompany(int input)
        {
            var result = new List<Vehicle_Vehicles>();
            try
            {
                var vehicles =  _vhcRepo.GetAllByColumnId("FK_CompanyID",input, true);
                result = vehicles.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicles_{ex.Message}_{input}");
            }
            return Task.FromResult(result);
        }
    }
}
