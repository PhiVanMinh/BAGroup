using Application.Dto.Users;
using Application.Interfaces;
using Application.IService;
using CachingFramework.Redis;
using CachingFramework.Redis.Contracts;
using CachingFramework.Redis.Contracts.RedisObjects;
using Infra_Persistence.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Common.Core.Entity;
using StackExchange.Redis;
using System.Text.Json;

namespace Infra_Persistence.Services
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/10/2023   created
    /// </Modified>
    [Authorize]
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IDatabase _cache;

        public UserService(
            IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IConfiguration configuration
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;

            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Kiểm tra thông tin user có hợp lệ không nếu không đưa ra thông báo. Ngược lại cập nhật thông tin xóa của bản ghi</summary>
        /// <param name="input">Thông tin cần xóa</param>
        /// <returns>Thông báo</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        public Task<string> DeleteUser(DeletedUserInput input)
        {
            try
            {
                var message = "";
                var users = _unitOfWork.UserRepository.GetAll().Where(e => input.ListId.Contains(e.UserId)).ToList();
                if (users.Count() == 0)
                {
                    message = "Can not find user !";
                    return Task.FromResult(message);
                }
                else
                {
                    if (input.ListId.Any(e => e == input.CurrentUserId))
                    {
                        message = $" Không thể xóa nhân viên {input.CurrentEmpName} !";
                        return Task.FromResult(message);
                    }
                    users.ForEach(e =>
                    {
                        e.IsDeleted = true;
                        e.DeletedUserId = input.CurrentUserId.ToString();
                        e.DeletedDate = DateTime.Now;
                    });
                    _unitOfWork.UserRepository.UpdateRange(users);
                    _unitOfWork.SaveAsync();
                }
                return Task.FromResult(message);
            } catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }

        }

        /// <summary>Lấy danh sách nhân viên theo giá trị lọc </summary>
        /// <param name="input">Giá trị tìm kiếm</param>
        /// <returns>Danh sách người dùng, thông báo</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        public async Task<PagedResultDto<GetAllUserDto>> GetAll(GetAllUserInput input)
        {
            string cacheKey = $"{DateTime.Now.ToString("dd_MM_yyyy_hh")} {input.Gender}_{input.FromDate}_{input.ToDate}_{input.UserName}_{input.FullName}_{input.Email}_{input.PhoneNumber}";

            try
            {
                List<GetAllUserDto> result = new List<GetAllUserDto>();
                var totalCount = 0;

                var cachedData =  _cache.KeyExists(cacheKey);
                if (cachedData)
                {
                    totalCount = (int)_cache.SortedSetLength($"{cacheKey}");
                    var redisData = _cache.SortedSetRangeByScore(cacheKey, (input.Page - 1) * input.PageSize, (input.Page) * input.PageSize - (input.Page == 1 ? 0 : 1));
                    result = redisData.Select(d => JsonSerializer.Deserialize<GetAllUserDto>(d)).ToList();
                }
                else
                {
                    var userList = await GetUserList(input);

                    totalCount = userList.Count();
                    result = userList.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize).ToList();
                    AddEnumerableToSortedSet(cacheKey, userList);
                    _cache.KeyExpire(cacheKey, DateTime.Now.AddMinutes(5));
                }

                return new PagedResultDto<GetAllUserDto>
                {
                    TotalCount = totalCount,
                    Result = result
                };

            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new PagedResultDto<GetAllUserDto>();
                return valueDefault;
            }

        }

        /// <summary>Thêm dữ liệu và redis cache</summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="key">Key cache để sau tìm kiếm</param>
        /// <param name="data">Danh sách dữ liệu cần lưu redis</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/22/2023   created
        /// </Modified>
        public void AddEnumerableToSortedSet<T>(string key, IEnumerable<T> data)
        {
                int i = 1;
                var context = new RedisContext(_configuration["RedisCacheUrl"]);
                IRedisSortedSet<T> sortedSet = context.Collections.GetRedisSortedSet<T>(key);
                sortedSet.AddRange(data.Select((m) => new SortedMember<T>(i, m)
                {
                    Value = m,
                    Score = i++
                }));
                context.Dispose();
        }

        private Task<List<GetAllUserDto>> GetUserList(GetAllUserInput input)
        {
            // Query using stored procedure

            //var result = await _unitOfWork.UserRepository.GetAllUsers(input);
            //return result;

            // Query using linq

            var result = _unitOfWork.UserRepository.GetAll().Where(e =>
                                            e.IsDeleted == false
                                            && (input.Gender > 0 ? input.Gender == e.Gender : true)
                                            && (input.FromDate != null ? e.CreateDate >= input.FromDate : true)
                                            && (input.ToDate != null ? e.CreateDate < input.ToDate.Value.AddDays(1) : true)
                                            && (string.IsNullOrWhiteSpace(input.UserName) || (e.UserName ?? "").StartsWith(input.UserName))
                                            && (string.IsNullOrWhiteSpace(input.FullName) || (e.EmpName ?? "").StartsWith(input.FullName))
                                            && (string.IsNullOrWhiteSpace(input.Email) || (e.Email ?? "").StartsWith(input.Email))
                                            && (string.IsNullOrWhiteSpace(input.PhoneNumber) || (e.PhoneNumber ?? "").StartsWith(input.PhoneNumber))
                                            ).OrderBy(e => e.CreateDate)
                                            .Select(user => new GetAllUserDto
                                            {
                                                UserId = user.UserId,
                                                BirthDay = user.BirthDay,
                                                PhoneNumber = user.PhoneNumber,
                                                EmpName = user.EmpName,
                                                UserName = user.UserName,
                                                Email = user.Email,
                                                Gender = user.Gender,
                                                UserType = user.UserType,
                                            });

            return Task.FromResult(result.ToList());

        }

        /// <summary>Thêm hoặc cập nhật thông tin người dùng</summary>
        /// <param name="user">The user.</param>
        /// <returns> Thông báo </returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        public async Task<string> CreateOrEditUser(CreateOrEditUserDto user)
        {
            try
            {
                var message = "";
                if (user.UserId != null && user.UserId != Guid.Empty) message = await UpdateUser(user);
                else message = await CreateUser(user);
                return message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Cập nhật thông tin user : 
        ///     + Kiểm tra thông tin người dùng cần cập nhật có tồn tại không nếu không đưa ra thông báo. 
        ///       Ngược lại nếu thỏa mãn thì cập nhật các thông tin cho người dùng cần cập nhật
        /// </summary>
        /// <param name="user">Thông tin người dùng cần cập nhật</param>
        /// <returns>Thông báo</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        [Authorize(Policy = Policies.UserUpdate)]
        private async Task<string> UpdateUser(CreateOrEditUserDto user)
        {
            var message = "";
            var userUpdate = _unitOfWork.UserRepository.FirstOrDefault(e => e.UserId == user.UserId);
            if (userUpdate != null)
            {
                userUpdate.EmpName = user.EmpName;
                userUpdate.Email = user.Email;
                userUpdate.BirthDay = user.BirthDay;
                userUpdate.Gender = user.Gender;
                userUpdate.PhoneNumber = user.PhoneNumber;
                userUpdate.LastModifyUserId = user.CurrentUserId.ToString();
                userUpdate.LastModifyDate = DateTime.Now;
                userUpdate.UserType = user.UserType;

                _unitOfWork.UserRepository.Update(userUpdate);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                message = $"Can not find user !";
            }
            return message;
        }

        /// <summary>
        /// Thêm mới thông tin user:
        ///  + Kiểm tra tên đăng nhập đã tồn tại chưa nếu rồi đưa ra thông báo lỗi và không tạo người dùng mới.
        ///    Ngược lại Mã hóa mật khẩu của người dùng , tạo thông tin người dùng mới
        /// </summary>
        /// <param name="user">Thông tin người dùng cần tạo mới</param>
        /// <returns>Thông báo</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        [Authorize(Policy = Policies.UserCreate)]
        private async Task<string> CreateUser(CreateOrEditUserDto user)
        {
            var message = "";
            var checkUser = _unitOfWork.UserRepository.FirstOrDefault(e => e.UserName == user.UserName);
            if (checkUser != null)
            {
                message = $"UserName is exist";
            } else
            { 
                // Mã hóa mật khẩu
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

                var newUser = new User
                {
                    EmpName = user.EmpName,
                    UserName = user.UserName,
                    Email = user.Email,
                    BirthDay = user.BirthDay,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    CreatorUserId = user.CurrentUserId.ToString(),
                    CreateDate = DateTime.Now,
                    PasswordHash = Convert.ToBase64String(passwordHash),
                    PasswordSalt = passwordSalt,
                    UserType = user.UserType ?? 2,
                };
                _unitOfWork.UserRepository.Add(newUser);
                await _unitOfWork.SaveAsync();
            }
            return message;
        }

        /// <summary>Tạo mật khẩu mã hóa</summary>
        /// <param name="password">Mật khẩu</param>
        /// <param name="passwordHash">Mật khẩu đã mã hóa băm</param>
        /// <param name="passwordSalt">Mật khẩu đã mã hóa</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/10/2023   created
        /// </Modified>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>Lấy dữ liệu để xuất excel</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Danh sách người dùng</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/17/2023   created
        /// </Modified>
        public async Task<List<GetAllUserDto>> GetDataToExportExcel(GetAllUserInput input)
        {
            try
            {
                string cacheKey = $"{DateTime.Now.ToString("dd_MM_yyyy_hh")} {input.Gender}_{input.FromDate}_{input.ToDate}_{input.UserName}_{input.FullName}_{input.Email}_{input.PhoneNumber}";
                List<GetAllUserDto> result = new List<GetAllUserDto>();
                var totalCount = 0;

                var cachedData = _cache.KeyExists(cacheKey);
                if (cachedData)
                {
                    totalCount = (int)_cache.SortedSetLength($"{cacheKey}");
                    var redisData = _cache.SortedSetRangeByScore(cacheKey);
                    result = redisData.Select(d => JsonSerializer.Deserialize<GetAllUserDto>(d)).ToList();
                }
                else
                {
                    var resultQuery = _unitOfWork.UserRepository.GetAll().Where(e =>
                                                e.IsDeleted == false
                                                && (input.Gender > 0 ? input.Gender == e.Gender : true)
                                                && (input.FromDate != null ? e.CreateDate >= input.FromDate : true)
                                                && (input.ToDate != null ? e.CreateDate < input.ToDate.Value.AddDays(1) : true)
                                                && (string.IsNullOrWhiteSpace(input.UserName) || (e.UserName ?? "").Contains(input.UserName))
                                                && (string.IsNullOrWhiteSpace(input.FullName) || (e.EmpName ?? "").Contains(input.FullName))
                                                && (string.IsNullOrWhiteSpace(input.Email) || (e.Email ?? "").Contains(input.Email))
                                                && (string.IsNullOrWhiteSpace(input.PhoneNumber) || (e.PhoneNumber ?? "").Contains(input.PhoneNumber))
                                                ).OrderBy(e => e.CreateDate)
                                                .Select(user => new GetAllUserDto
                                                {
                                                    UserId = user.UserId,
                                                    BirthDay = user.BirthDay,
                                                    PhoneNumber = user.PhoneNumber,
                                                    EmpName = user.EmpName,
                                                    UserName = user.UserName,
                                                    Email = user.Email,
                                                    Gender = user.Gender,
                                                    UserType = user.UserType,
                                                });
                    result = resultQuery.ToList();

                    // Query using stored procedure
                    //result = await _unitOfWork.UserRepository.GetAllUsers(input);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new List<GetAllUserDto>();
                return valueDefault;
            }
        }

    }
}
