using Application.Dto.Common;
using Application.Dto.Users;
using Application.IService;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Master;
using Infra_Persistence.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using System.Drawing;
using System.Text;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly ILogger<UsersController> _logger;
        private readonly IDistributedCache _cache;

        public UsersController
            (
            IUserService user,
            ILogger<UsersController> logger,
            IDistributedCache cache
            )
        {
            _user = user;
            _logger = logger;
            _cache = cache;
        }

        #region -- Lấy danh sách user
        /// <summary>Đưa ra danh sách người dùng. Nếu hệ thống không thể truy cập đưa ra giá trị mặc định và thông báo</summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns>Thông báo, trạng thái, danh sách người dùng</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        [HttpPost("employees")]
        [Authorize(Policy = Policies.UserView)]
        public async Task<IActionResult> GetAll(GetAllUserInput input)
        {
            var respon = new ResponDto<PagedResultDto>();
            var inputToString = JsonSerializer.Serialize<GetAllUserInput>(input);

            if (input.Page == 0 || input.PageSize == 0)
            {
                return BadRequest("Vui lóng nhập đủ số trang và kích thước trang! ");
            }
            try
            {
                var userList = await _user.GetAll(input);
                respon.Result = userList;

            } catch (Exception ex)
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

        #region -- Thêm hoặc cập nhật thông tin user
        /// <summary>Api thêm hoặc sửa thông tin user</summary>
        /// <param name="user">Thông tin người dùng cần tạo hoặc cập nhật</param>
        /// <returns> Thông báo và trạng thái</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023     created
        /// </Modified>
        [HttpPost("createOrEdit-employees")]
        [Authorize(Policy = Policies.CreateOrUpdateUser)]
        public async Task<IActionResult> CreateOrEditUsers([FromBody] CreateOrEditUserDto user)
        {
            var respon = new ResponDto<bool>();
            if (user.CurrentUserId == null || user.CurrentUserId == Guid.Empty)
            {
                return BadRequest("Mã người dùng đang đăng nhập không hợp lệ !");
            }
            if (((user.Password ?? "").Length < 6 || (user.Password ?? "").Length > 100) && (user.UserId == null || user.UserId == Guid.Empty)) return BadRequest("Mật Khẩu phải ít nhất 6 kí tự và nhiều nhất 100 kí tự !");
            if (user.BirthDay == null || (user.BirthDay > DateTime.Now.AddYears(-18))) { return BadRequest("Người dùng chưa đủ 18 tuổi !"); }

            try
            {
                var message = await _user.CreateOrEditUser(user);
                if (!string.IsNullOrEmpty(message)) 
                {
                    respon.Message = message;
                    respon.StatusCode = 400;
                }
            } catch (Exception ex)
            {
                respon.StatusCode = 500;
                respon.Message = ex.Message;
                var inputToString = JsonSerializer.Serialize<CreateOrEditUserDto>(user);
                _logger.LogInformation($" {respon.Message} InputValue: {inputToString}");

            }
            return Ok(respon);
        }
        #endregion

        #region -- xóa thông tin user
        /// <summary>Api xóa thông tin người dùng</summary>
        /// <param name="input">Thông tin người xóa , danh sách người dùng cần xóa</param>
        /// <returns>Thông báo và trạng thái</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        [HttpPost("delete-employees")]
        [Authorize(Policy = Policies.UserDelete)]
        public async Task<IActionResult> DeleteUsers([FromBody] DeletedUserInput input)
        {
            if (input.ListId != null && input.ListId.Count() == 0) 
            {
                return BadRequest("Vui lòng chọn 1 bản ghi để xóa !");
            }
            if (input.CurrentUserId == null)
            {
                return BadRequest("Mã người dùng đang đăng nhập không hợp lệ !");
            }
            var respon = new ResponDto<bool>();
            try
            {
                var message = await _user.DeleteUser(input);
                if (!string.IsNullOrEmpty(message))
                {
                    respon.Message = message;
                    respon.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                respon.StatusCode = 500;
                respon.Message = ex.Message;
                var inputToString = JsonSerializer.Serialize<DeletedUserInput>(input);
                _logger.LogInformation($" {respon.Message} InputValue: {inputToString}");
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
        [HttpPost("export-excel")]
        public async Task<ActionResult> ExportToExcel(GetAllUserInput input)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var fileName = $"List_User_{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}";

                    var users = await _user.GetDataToExportExcel(input);

                    var dataTable = GetTable("Users", users);

                    var worksheet = workbook.Worksheets.Add(dataTable);
                    worksheet.Columns("A").Width = 10;
                    worksheet.Columns("B").Width = 35;
                    worksheet.Columns("C").Width = 20;
                    worksheet.Columns("D").Width = 15;
                    worksheet.Columns("E").Width = 10;
                    worksheet.Columns("F").Width = 20;
                    worksheet.Columns("G").Width = 40;

                    worksheet.Range($"A1:G1").Style.Fill.BackgroundColor = XLColor.Gray;

                    worksheet.Range($"A1:G{users.Count() + 1}").Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin);

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
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
        private DataTable GetTable(String tableName, List<GetAllUserDto> userList)
        {
            DataTable table = new DataTable();
            table.TableName = tableName;
            table.Columns.Add("STT", typeof(int));
            table.Columns.Add("FullName", typeof(string));
            table.Columns.Add("UserName", typeof(string));
            table.Columns.Add("BirthDay", typeof(DateTime));
            table.Columns.Add("Gender", typeof(string));
            table.Columns.Add("PhoneNumber", typeof(string));
            table.Columns.Add("Email", typeof(string));

            var count = 1;
            foreach (var user in userList)
            {
                table.Rows.Add(count, user.EmpName, user.UserName, user.BirthDay, user.Gender == 1 ? "Nam" : (user.Gender == 2 ? "Nữ" : "Khác"), user.PhoneNumber, user.Email);
                count++;
            }
            return table;
        }
        #endregion
    }
}
