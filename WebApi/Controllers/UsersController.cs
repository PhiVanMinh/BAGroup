using Application.Dto.Common;
using Application.Dto.Users;
using Application.IService;
using Domain.Master;
using Infra_Persistence.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Drawing;
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
                var inputToString = JsonSerializer.Serialize<GetAllUserInput>(input);
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
            if (user.CurrentUserId == null)
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
    }
}
