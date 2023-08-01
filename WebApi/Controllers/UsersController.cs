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

        public UsersController(IUserService user)
        {
            _user = user;
        }
        // Lấy danh sách user
        [HttpPost("employees")]
        public async Task<PagedResultDto> GetAll(GetAllUserInput input)
          => await _user.GetAll(input);

        // Thêm hoặc cập nhật thông tin user
        [HttpPost("createOrEdit-employees")]
        public async Task<IActionResult> CreateOrEditUsers([FromBody] CreateOrEditUserDto user)
        {
            try
            {
                await _user.CreateOrEditUser(user);
                return Ok(true);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // xóa thông tin user
        [HttpPost("delete-employees")]
        public async Task<IActionResult> DeleteUsers([FromBody] DeletedUserInput input)
        {
            
            try
            {
                await _user.DeleteUser(input);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
