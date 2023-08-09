using Application.Dto.Users;
using Application.IService;
using Infrastructures.Authorization;
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
        [Authorize(Policy = Policies.UserView)]
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
        [Authorize(Policy = Policies.CreateOrUpdateUser)]
        public async Task<IActionResult> CreateOrEditUsers([FromBody] CreateOrEditUserDto user)
        {
            if (user.CurrentUserId == null)
            {
                return BadRequest("Mã người dùng đang đăng nhập không hợp lệ !");
            }
            if(((user.Password ?? "").Length < 6 || (user.Password ?? "").Length > 100) && user.UserId == null) return BadRequest("Mật Khẩu phải ít nhất 6 kí tự và nhiều nhất 100 kí tự !");
            if(user.BirthDay == null || (user.BirthDay > DateTime.Now.AddYears(-18))) { return BadRequest("Người dùng chưa đủ 18 tuổi !"); }

            var respon = await _user.CreateOrEditUser(user);
            if (!string.IsNullOrEmpty(respon.Message)) _logger.LogInformation(respon.Message);
            return Ok(respon);
        }
        #endregion

        #region xóa thông tin user
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
            var respon = await _user.DeleteUser(input);
            if (!string.IsNullOrEmpty(respon.Message)) _logger.LogInformation(respon.Message);
            return Ok(respon);
        }
        #endregion
    }
}
