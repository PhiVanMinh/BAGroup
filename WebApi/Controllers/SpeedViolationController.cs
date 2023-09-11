using Application.Dto.Common;
using Application.Dto.SpeedViolation;
using Application.Dto.Users;
using Application.IService;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Infra_Persistence.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Nest;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace WebApi.Controllers
{
        /// <summary>Các API cho chức năng báo cáo vi phạm tốc độ</summary>
        /// <Modified>
        /// Name        Date        Comments
        /// minhpv    9/05/2023     created
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
        /// minhpv    9/05/2023     created
        /// </Modified>
        [HttpPost("list-vehicle")]
        [Authorize(Policy = Policies.UserView)]
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
        /// minhpv    9/05/2023     created
        /// </Modified>
        [HttpPost("speed-violation")]
        [Authorize(Policy = Policies.UserView)]
        public async Task<IActionResult> GetSpeedViolationVehicleList(SpeedViolationVehicleInput input)
            {
                var respon = new ResponDto<PagedResultDto<GetAllSpeedViolationVehicleDto>>();
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
                    respon.Result = new PagedResultDto<GetAllSpeedViolationVehicleDto>
                    {
                        TotalCount = 0,
                        Result = new List<GetAllSpeedViolationVehicleDto>()
                    };
                }
                return Ok(respon);
            }
        #endregion

        #region -- Export excel
        /// <summary>Xuất báo cáo excel</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>File excel</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/08/2023   created
        /// </Modified>
        [HttpPost("report_speed_violation")]
        public async Task<ActionResult> ExportToExcel(SpeedViolationVehicleInput input)
        {
            try
            {
                var xmlFile = Path.Combine(Environment.CurrentDirectory, @"ReportFile\report_speed_violation.xlsx");
                using (var workBook = new XLWorkbook(xmlFile))
                {
                    var fileName = $"Speed_Violation_Report_{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}";

                    var data = await _speedViolation.GetDataToExportExcel(input);


                    var workSheet = workBook.Worksheet(1);
                    var firstRowUsed = workSheet.FirstRowUsed();
                    var firstPossibleAddress = workSheet.Row(firstRowUsed.RowNumber()).FirstCell().Address;
                    var lastPossibleAddress = workSheet.LastCellUsed().Address;

                    IXLCell cellForNewData = workSheet.Cell(workSheet.LastRowUsed().RowNumber() + 1, 1);

                    var dt = ConvertListToDataTable(data);
                    cellForNewData.InsertData(dt.Rows);

                    workSheet.Range($"A1:P{data.Count() + 2}").Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin);

                    using (var stream = new MemoryStream())
                    {
                        workBook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName + ".xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($" {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Insert data to table</summary>
        /// <param name="data"> Dữ liệu cần xuất excel</param>
        /// <returns>DataTable</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    09/08/2023   created
        /// </Modified>
        private static DataTable ConvertListToDataTable(List<GetAllSpeedViolationVehicleDto> data)
        {
            var table = new DataTable("SpeedViolation");
            table.TableName = "SpeedViolation";
            table.Columns.Add("STT", typeof(string));
            table.Columns.Add("PlateNo", typeof(string));
            table.Columns.Add("PrivateCode", typeof(string));
            table.Columns.Add("TransportType", typeof(string));
            table.Columns.Add("SpeedVioLevel1", typeof(int));
            table.Columns.Add("SpeedVioLevel2", typeof(int));
            table.Columns.Add("SpeedVioLevel3", typeof(int));
            table.Columns.Add("SpeedVioLevel4", typeof(int));
            table.Columns.Add("TotalSpeedVio", typeof(int));
            table.Columns.Add("RatioSpeedVio", typeof(float));
            table.Columns.Add("TotalKmVio", typeof(float));
            table.Columns.Add("TotalKm", typeof(string));
            table.Columns.Add("RatioKmVio", typeof(float));
            table.Columns.Add("TotalTimeVio", typeof(string));
            table.Columns.Add("TotalTime", typeof(string));
            table.Columns.Add("RatioTimeVio", typeof(float));

            var count = 1;
            foreach (var vehicle in data)
            {
                table.Rows.Add(count, vehicle.VehiclePlate, vehicle.PrivateCode, vehicle.TransportType, vehicle.SpeedVioLevel1, vehicle.SpeedVioLevel2,
                    vehicle.SpeedVioLevel3, vehicle.SpeedVioLevel4, vehicle.TotalSpeedVio, vehicle.RatioSpeedVio, Math.Round((decimal)(vehicle.TotalKmVio ?? 0),2), Math.Round((decimal)(vehicle.TotalKm ?? 0), 2),
                    vehicle.TotalKm != null ? Math.Round(((decimal)(vehicle.TotalKmVio ?? 0) * 100 / (decimal)vehicle.TotalKm), 2) : 0,
                    formatMinuteToHourMinute(vehicle.TotalTimeVio ?? 0), formatMinuteToHourMinute(vehicle.TotalTime ?? 0),
                    vehicle.TotalTime != null ? Math.Round((decimal)((decimal)(vehicle.TotalTimeVio ?? 0) * 100 / vehicle.TotalTime), 2) : 0);
                count++;
            }
            return table;
        }

        /// <summary>Định dạng giá trị phút về hh:mm </summary>
        /// <param name="input">tổng số phút</param>
        /// <returns>Định dạng hh:mm </returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    09/11/2023   created
        /// </Modified>
        private static string formatMinuteToHourMinute(int input)
        {
            var hour = (input - (input % 60)) / 60;
            var minute = input % 60;
            return $"{(hour < 10 ? 0 : "")}{hour}:{(minute < 10 ? 0 : "")}{minute}";
        }
        #endregion
    }
}
