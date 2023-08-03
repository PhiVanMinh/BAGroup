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
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _repo;

        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController
            (
                IAuthService repo,
                IMapper mapper,
                ILogger<AuthController> logger
            )
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
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
            
            var user = _mapper.Map<UserLoginInfo>(userFromRepo.User);

            // Gen token cho phiên đăng nhập

            string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
            var issuer = "http://mysite.com";  //normally this will be your site URL    

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            foreach (var role in userFromRepo.Roles)
            {
                permClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
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
