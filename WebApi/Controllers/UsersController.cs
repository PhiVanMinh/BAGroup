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

        [HttpPost("employees")]
        public async Task<PagedResultDto> GetAll(GetAllUserInput input)
          => await _user.GetAll(input);


        [HttpPost("createOrEdit-employees")]
        public async Task CreateOrEditUsers([FromBody] CreateOrEditUserDto user)
        {
            await _user.CreateOrEditUser(user);
        }

        [HttpPost("delete-employees")]
        public async Task CreateOrEditUsers([FromBody] DeletedUserInput input)
        {
            await _user.DeleteUser(input);
        }
    }
}
