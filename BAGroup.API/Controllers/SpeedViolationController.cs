using Application.Dto.Common;
using Application.Dto.SpeedViolation;
using Application.Dto.Users;
using Application.IService;
using ClosedXML.Excel;
using Infra_Persistence.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel;
using System.Data;
using System.Text.Json;

namespace BAGroup.API.Controllers
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
        //[Authorize(Policy = Policies.UserView)]
        public async Task<IActionResult> GetVehicleByCompanyId(int input)
        {
            List<GetVehicleListDto> result = new List<GetVehicleListDto>();
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
        //[Authorize(Policy = Policies.UserView)]
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
                var xmlFile = Path.Combine(Environment.CurrentDirectory, @"ReportFile/report_speed_violation.xlsx");
                using (var workBook = new XLWorkbook(xmlFile))
                {
                    var fileName = $"Speed_Violation_Report_{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}";

                    var data = await _speedViolation.GetDataToExportExcel(input);

                    var workSheet = workBook.Worksheet(1);
                    var firstRowUsed = workSheet.FirstRowUsed();
                    var firstPossibleAddress = workSheet.Row(firstRowUsed.RowNumber()).FirstCell().Address;
                    var lastPossibleAddress = workSheet.LastCellUsed().Address;

                    workSheet.Cell(1, 1).Value = $"BÁO CÁO VI PHẠM TỐC ĐỘ THEO ĐƠN VỊ VẬN TẢI TỪ {input.FromDate.Value.ToString("dd/MM/yyyy")} ĐẾN {input.ToDate.Value.ToString("dd/MM/yyyy")}";

                    IXLCell cellForNewData = workSheet.Cell(workSheet.LastRowUsed().RowNumber() + 1, 1);

                    var dt = LinqHelper.ToDataTable(data);
                    cellForNewData.InsertData(dt.Rows);

                    workSheet.Name = "BCVP";
                    workSheet.Range($"A2:P{data.Count() + 3}").Style
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
        #endregion
    }

    /// <summary>Class chung để chuyển DL</summary>
    /// <Modified>
    /// Name        Date        Comments
    /// minhpv    10/17/2023     created
    /// </Modified>
    public static class LinqHelper
    {
        /// <summary>Chuyển DL dạng List sang DataTable</summary>
        /// <param name="data">Danh sách dữ liệu</param>
        /// <returns>DataTable</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    10/17/2023   created
        /// </Modified>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            table.Columns.Add("Num", typeof(string));
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            var count = 1;
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                row["Num"] = count;
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
                count++;
            }
            return table;
        }
    }
}

