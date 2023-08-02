using Application.Dto.Users;
using Application.IService;
using Domain.Master;
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

        // Lấy danh sách user
        [HttpPost("employees")]
        public async Task<IActionResult> GetAll(GetAllUserInput input)
        {
            var respon = await _user.GetAll(input);
            if (!string.IsNullOrEmpty(respon.Message)) _logger.LogInformation(respon.Message);
            return Ok(respon);
        }

        // Thêm hoặc cập nhật thông tin user
        [HttpPost("createOrEdit-employees")]
        public async Task<IActionResult> CreateOrEditUsers([FromBody] CreateOrEditUserDto user)
        {
            var respon = await _user.CreateOrEditUser(user);
            if (!string.IsNullOrEmpty(respon.Message)) _logger.LogInformation(respon.Message);
            return Ok(respon);
        }

        // xóa thông tin user
        [HttpPost("delete-employees")]
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
    }
}
