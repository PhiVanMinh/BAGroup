using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using VehicleInformation.Dto;
using VehicleInformation.Interfaces.IService;
using VehicleInformation.Interfaces.IRepository;
using VehicleInformation.Models;

namespace VehicleInformation.Controllers
{
    /// <summary>Các API lấy thông tin báo cáo xe vi phạm tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/18/2023   created
    /// </Modified>
    [ApiController]
    [Route("[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleTransportTypesService _vehicleTransportTypes;
        private readonly ITransportTypesService _transportTypes;
        private readonly IVehiclesService _vehicles;
        private readonly IActivitySummariesService _atvSum;
        private readonly ISpeedOversService _speedOversService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController
            (
            IVehicleTransportTypesService vehicleTransportTypes,
            ITransportTypesService transportTypes,
            IVehiclesService vehicles,
            IActivitySummariesService atvSum,
            ISpeedOversService speedOversService,
            ILogger<VehiclesController> logger
            )
        {
            _vehicleTransportTypes = vehicleTransportTypes;
            _transportTypes = transportTypes;
            _vehicles = vehicles;
            _atvSum = atvSum;
            _speedOversService = speedOversService;
            _logger = logger;
        }

        #region -- Lấy danh sách xe theo công ty
        /// <summary>Đưa ra danh sách xe theo công ty. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns>Thông báo, trạng thái, danh sách xe theo công ty</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/05/2023     created
        /// </Modified>
        [HttpPost("vehicle")]
        public async Task<IActionResult> GetVehicleByCompanyId(int input)
        {
            List<Vehicle_Vehicles> result = new List<Vehicle_Vehicles>();
            var inputToString = JsonSerializer.Serialize<int>(input);

            try
            {
               result = await _vehicles.GetAllByCompany(input);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region -- Lấy danh sách loại xe theo công ty
        /// <summary>Đưa ra danh sách loại xe theo công ty. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// <returns>Thông báo, trạng thái, danh sách xe theo công ty</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/05/2023     created
        /// </Modified>
        [HttpPost("transport-type")]
        public async Task<IActionResult> GetTransportType()
        {
            List<BGT_TranportTypes> result = new List<BGT_TranportTypes>();

            try
            {
                result = await _transportTypes.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Ok(result);
        }
        #endregion


        #region -- Lấy danh sách loại xe của từng xe theo theo công ty
        /// <summary>Đưa ra danh sách loại xe của từng xe theo công ty. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// <returns>Thông báo, trạng thái, danh sách xe theo công ty</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/05/2023     created
        /// </Modified>
        [HttpPost("vehicle-type")]
        public async Task<IActionResult> GetVehicleTransportType()
        {
            List<BGT_VehicleTransportTypes> result = new List<BGT_VehicleTransportTypes>();

            try
            {
                result = await _vehicleTransportTypes.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region -- Lấy thông tin vi phạm tốc độ của các xe theo theo công ty
        /// <summary>Đưa ra thông tin vi phạm tốc độ của các xe theo công ty. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns>Thông báo, trạng thái, danh sách xe theo công ty</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/05/2023     created
        /// </Modified>
        [HttpPost("speedOver")]
        public async Task<IActionResult> GetSpeedOvers(DateTime fromDate, DateTime toDate)
        {
            List<BGT_SpeedOvers> result = new List<BGT_SpeedOvers>();

            try
            {
                result = await _speedOversService.GetAllSpeedOversByDate(fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region -- Lấy thông tin tổng hợp của các xe theo theo công ty
        /// <summary>Đưa ra thông tin tổng hợp của các xe theo công ty. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// <returns>Thông báo, trạng thái, danh sách xe theo công ty</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/05/2023     created
        /// </Modified>
        [HttpPost("activiti-summary")]
        public async Task<IActionResult> GetActivitySummaries(int input)
        {
            List<Report_ActivitySummaries> result = new List<Report_ActivitySummaries>();

            try
            {
                result = await _atvSum.GetAllByCompany(input);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Ok(result);
        }
        #endregion
    }
}
