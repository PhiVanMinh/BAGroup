using Application.Dto.Common;
using Application.Dto.SpeedViolation;
using Application.Dto.Users;
using Application.IService;
using Infra_Persistence.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Nest;
using System.Text.Json;

namespace WebApi.Controllers
{
        /// <summary>Các API cho chức năng quản lý người dùng</summary>
        /// <Modified>
        /// Name        Date        Comments
        /// minhpv    8/10/2023     created
        /// </Modified>
        [ApiController]
        [Route("[controller]")]
        public class SpeedViolationController : ControllerBase
        {
            private readonly ISpeedViolationService _speedViolation;
            private readonly ILogger<SpeedViolationController> _logger;
            private readonly IDistributedCache _cache;

            public SpeedViolationController
                (
                ISpeedViolationService speedViolation,
                ILogger<SpeedViolationController> logger,
                IDistributedCache cache
                )
            {
                _speedViolation = speedViolation;
                _logger = logger;
                _cache = cache;
            }

        #region -- Lấy danh sách xe theo công ty
        /// <summary>Đưa ra danh sách xe theo công ty. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns>Thông báo, trạng thái, danh sách xe theo công ty</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        [HttpPost("list-vehicle")]
        //[Authorize(Policy = Policies.UserView)]
        public async Task<IActionResult> GetVehicleByCompanyId(int input)
        {
            List<GetVehicleListDto> result = new List<GetVehicleListDto> ();
            var inputToString = JsonSerializer.Serialize<int>(input);

            try
            {
              result = await _speedViolation.GetVehicleByCompanyId(input);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region -- Lấy danh sách xe vi phạm tốc độ
        /// <summary>Đưa ra danh sách xe vi phạm tốc độ. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns>Thông báo, trạng thái, danh sách  xe vi phạm tốc độ</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        [HttpPost("speed-violation")]
            //[Authorize(Policy = Policies.UserView)]
            public async Task<IActionResult> GetSpeedViolationVehicleList(SpeedViolationVehicleInput input)
            {
                var respon = new ResponDto<PagedResultDto>();
                var inputToString = JsonSerializer.Serialize<SpeedViolationVehicleInput>(input);

                if (input.Page == 0 || input.PageSize == 0)
                {
                    return BadRequest("Vui lòng nhập đủ số trang và kích thước trang! ");
                }
                try
                {
                    var result = await _speedViolation.GetAllSpeedViolationVehicle(input);
                    respon.Result = result;

                }
                catch (Exception ex)
                {
                    respon.StatusCode = 500;
                    respon.Message = ex.Message;
                    _logger.LogInformation($" {respon.Message} InputValue: {inputToString}");
                    respon.Result = new PagedResultDto
                    {
                        TotalCount = 0,
                        Result = new List<GetAllUserDto>()
                    };
                }
                return Ok(respon);
            }
            #endregion
        }
    }
