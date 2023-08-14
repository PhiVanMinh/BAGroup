using Application.Dto.Common;
using Application.Dto.Users;
using Application.Enum;
using Application.Interfaces;
using Application.IService;
using Domain.Master;
using Infra_Persistence.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Infra_Persistence.Services
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/10/2023   created
    /// </Modified>
    [Authorize]
    public class UserService : IUserService
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            ILogger<UserService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
        public Task<PagedResultDto> GetAll(GetAllUserInput input)
        {
            try
            {
                var result = _unitOfWork.UserRepository.GetAll().Where(e =>
                                                e.IsDeleted == false
                                                && input.Gender > 0 ? input.Gender == e.Gender : true
                                                && input.FromDate != null ? e.BirthDay >= input.FromDate : true
                                                && input.ToDate != null ? e.BirthDay < input.ToDate.Value.AddDays(1) : true
                                                && ( string.IsNullOrWhiteSpace(input.ValueFilter) ? true
                                                    : input.TypeFilter == FilterType.UserName ? (e.UserName ?? "").Contains(input.ValueFilter)
                                                        : (input.TypeFilter == FilterType.FullName ? (e.EmpName ?? "").Contains(input.ValueFilter)
                                                            : (input.TypeFilter == FilterType.Email ? (e.Email ?? "").Contains(input.ValueFilter)
                                                                : (e.PhoneNumber ?? "").Contains(input.ValueFilter)
                                                        )))
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
                var totalCount = result.Count();
                var pageAndFilterResult = result.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize);
                return Task.FromResult(
                        new PagedResultDto
                        {
                            TotalCount = totalCount,
                            Result = pageAndFilterResult.ToList()
                        }
                    );
            } catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new PagedResultDto();
                return Task.FromResult( valueDefault );
            }
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

    }
}
