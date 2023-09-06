using Application.Dto.Common;
using Application.Dto.Users;
using Application.Enum;
using Application.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Drawing;
using WebApi.Controllers;

namespace WebApi_Test
{
    /// <summary>Unit test api user</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/15/2023   created
    /// </Modified>
    public class UserTest
    {
        private readonly Mock<IUserService> userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IDistributedCache _cache;

        public UserTest()
        {
            userService = new Mock<IUserService>();
        }
        /// <summary>Test get all function.</summary>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/15/2023   created
        /// </Modified>
        [Fact]
        public async void Test_GetAll()
        {
            //arrange
            var input = new GetAllUserInput();

            var result = new PagedResultDto<GetAllUserDto>
            {
                TotalCount = 1,
                Result = new List<GetAllUserDto>{
                    new GetAllUserDto
                    {
                       UserId = Guid.Parse("1840E8AA-846D-466F-330A-08DB9D72BA68"),
                       BirthDay = DateTime.Parse("2023-08-15 16:33:48.207"),
                       Email = "minhpv@gmail.com",
                       EmpName = "Phí Văn Minh",
                       Gender = 1,
                       PhoneNumber = "0999999998",
                       UserName = "minhpv",
                       UserType = 1,
                    }
                }
            };

            var respon1 = new ResponDto<PagedResultDto<GetAllUserDto>>();

            var userList = new PagedResultDto<GetAllUserDto>();
                userService.Setup(x => x.GetAll(input))
                .Returns(Task.FromResult(result));
            var userController = new UsersController(userService.Object, _logger, _cache);

            //act
            IActionResult response = await userController.GetAll(input);

            ObjectResult objectResponse = Assert.IsType<OkObjectResult>(response);
            ResponDto<PagedResultDto<GetAllUserDto>>? userResult = objectResponse != null ? (ResponDto<PagedResultDto<GetAllUserDto>>?)objectResponse.Value : new ResponDto<PagedResultDto<GetAllUserDto>>();

            //assert
            Assert.NotNull(userResult);
            Assert.True(result.TotalCount.Equals((userResult ?? new ResponDto<PagedResultDto<GetAllUserDto>>()).Result.TotalCount));
            Assert.True(result.Result.Equals((userResult ?? new ResponDto<PagedResultDto<GetAllUserDto>>()).Result.Result));
        }

    }
}