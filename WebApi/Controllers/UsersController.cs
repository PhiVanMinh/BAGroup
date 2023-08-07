using Application.Dto.Users;
using Application.IService;
using Domain.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly ILogger<UsersController> _logger;

        public UsersController
            (
            IUserService user,
            ILogger<UsersController> logger
            )
        {
            _user = user;
            _logger = logger;
        }

        #region -- Lấy danh sách user
        [HttpPost("employees")]
        public async Task<IActionResult> GetAll(GetAllUserInput input)
        {
            if (input.Page == 0 || input.PageSize == 0)
            {
                return BadRequest("Vui lóng nhập đủ số trang và kích thước trang! ");
            }
            var respon = await _user.GetAll(input);
            if (!string.IsNullOrEmpty(respon.Message)) _logger.LogInformation(respon.Message);
            return Ok(respon);
        }
        #endregion

        #region Thêm hoặc cập nhật thông tin user
        [HttpPost("createOrEdit-employees")]
        public async Task<IActionResult> CreateOrEditUsers([FromBody] CreateOrEditUserDto user)
        {
            //if (user.UserId == null)
            //{
            //    return BadRequest("Mã người dùng cần cập nhật không hợp lệ !");
            //}
            var respon = await _user.CreateOrEditUser(user);
            if (!string.IsNullOrEmpty(respon.Message)) _logger.LogInformation(respon.Message);
            return Ok(respon);
        }
        #endregion

        #region xóa thông tin user
        [HttpPost("delete-employees")]
        //[Authorize(Policy = "DeleteAccess")]

        public async Task<IActionResult> DeleteUsers([FromBody] DeletedUserInput input)
        {
            if (input.ListId != null && input.ListId.Count() == 0) 
            {
                return BadRequest("Vui lòng chọn 1 bản ghi để xóa !");
            }
            var respon = await _user.DeleteUser(input);
            if (!string.IsNullOrEmpty(respon.Message)) _logger.LogInformation(respon.Message);
            return Ok(respon);
        }
        #endregion
    }
}
