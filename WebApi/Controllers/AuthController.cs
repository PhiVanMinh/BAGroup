using Application.Dto.Users;
using Application.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers
{
    /// <summary>Api cho chức năng đăng nhập</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/10/2023   created
    /// </Modified>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _repo;

        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;

        public AuthController
            (
                IAuthService repo,
                IMapper mapper,
                ILogger<AuthController> logger,
                IConfiguration config
            )
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        /// <summary>Api đăng nhập</summary>
        /// <param name="userForLoginDto">Thông tin đăng nhập </param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // Kiểm  tra dữ liệu đăng nhập đầu vào
            if( string.IsNullOrWhiteSpace(userForLoginDto.UserName)|| string.IsNullOrWhiteSpace(userForLoginDto.Password))
            {
                _logger.LogInformation("UserName or Password incorrect");
                return Unauthorized();
            }    
            var userFromRepo = await _repo.Login(userForLoginDto);

            // Kiểm tra thông tin đăng nhập
            if (userFromRepo.User == null)
            {
                _logger.LogInformation("UserName or Password incorrect");
                return Unauthorized();
            };
            
            // Map thông tin user
            var user = _mapper.Map<UserLoginInfo>(userFromRepo.User);

            // Gen token cho phiên đăng nhập

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Auth0:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Khởi tạo danh sách Claims 
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            foreach (var role in userFromRepo.Roles)
            {
                permClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Tạo Token dựa trên các tham số 
            var token = new JwtSecurityToken(
                            issuer: _config["Auth0:Issuer"],
                            audience: _config["Auth0:Audience"],
                            permClaims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

            user.Token = jwt_token;

            return Ok( new
            {
                jwt_token,
                user
            });

        }
    }
}
