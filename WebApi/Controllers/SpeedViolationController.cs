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
        /// minhpv    8/17/2023   created
        /// </Modified>
        [HttpPost("report_speed_violation")]
        public async Task<ActionResult> ExportToExcel(SpeedViolationVehicleInput input)
        {
            try
            {
                var xmlFile = Path.Combine(Environment.CurrentDirectory, @"ReportFile\report_speed_violation.xlsx");
                //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"report_speed_violation.xlsx");
                using (var workBook = new XLWorkbook(xmlFile))
                {
                    var fileName = $"List_User_{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}";

                    var users = await _speedViolation.GetDataToExportExcel(input);

                    //var dataTable = GetTable("Users", users);

                    var workSheet = workBook.Worksheet(1);
                    var firstRowUsed = workSheet.FirstRowUsed();
                    var firstPossibleAddress = workSheet.Row(firstRowUsed.RowNumber()).FirstCell().Address;
                    var lastPossibleAddress = workSheet.LastCellUsed().Address;

                    IXLCell cellForNewData = workSheet.Cell(workSheet.LastRowUsed().RowNumber() + 1, 1);

                    var dt = ConvertListToDataTable(users);
                    cellForNewData.InsertData(dt.Rows);

                    workSheet.Range($"A1:G{users.Count() + 1}").Style
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
        /// <param name="tableName">Name of the table.</param>
        /// <param name="userList">The user list.</param>
        /// <returns>DataTable</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/31/2023   created
        /// </Modified>
        private static DataTable ConvertListToDataTable(List<GetAllSpeedViolationVehicleDto> userList)
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
            table.Columns.Add("TotalTimeVio", typeof(string));
            table.Columns.Add("TotalTime", typeof(string));

            var count = 1;
            foreach (var user in userList)
            {
                table.Rows.Add(count, user.VehiclePlate, user.PrivateCode, user.TransportType, user.SpeedVioLevel1, user.SpeedVioLevel2,
                    user.SpeedVioLevel3, user.SpeedVioLevel4, user.TotalSpeedVio, user.RatioSpeedVio, user.TotalKmVio, user.TotalKm, user.TotalTimeVio, user.TotalTime);
                count++;
            }
            return table;
        }
        #endregion
    }
}
