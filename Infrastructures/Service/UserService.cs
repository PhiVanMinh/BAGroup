using Application.Dto.Common;
using Application.Dto.Users;
using Application.Interfaces;
using Application.IService;
using Domain.Master;
using System.Threading.Tasks;

namespace Infrastructures.Service
{
    public class UserService : IUserService
    {
        public IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region -- Xóa thông tin user
        public Task<ResponDto<bool>> DeleteUser(DeletedUserInput input)
        {
            var respon = new ResponDto<bool>();
            try
            {
                var users = _unitOfWork.UserRepository.GetAll().Where(e => input.ListId.Contains(e.Id)).ToList();
                if (users.Count() == 0)
                {
                    respon.StatusCode = 400;
                    respon.Message = "Can not find user !";
                    return Task.FromResult(respon);
                }
                else
                {
                    if (input.ListId.Any(e => e == input.CurrentUserId))
                    {
                        respon.StatusCode = 400;
                        respon.Message = $" Không thể xóa nhân viên {input.CurrentUserName} !";
                        return Task.FromResult(respon);
                    }
                    users.ForEach(e =>
                    {
                        e.IsDeleted = true;
                        e.DeletedUserId = input.CurrentUserId;
                        e.LastModifyDate = DateTime.Now;
                    });
                    _unitOfWork.UserRepository.UpdateRange(users);
                    _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                respon.StatusCode = 500;
                respon.Message = ex.Message;
            }
            return Task.FromResult(respon);

        }
        #endregion

        #region -- Lấy danh sách thông tin user 
        public Task<ResponDto<PagedResultDto>> GetAll(GetAllUserInput input)
        {
            var respon = new ResponDto<PagedResultDto>();
            try
            {
                var result = from user in _unitOfWork.UserRepository.GetAll()
                                    .Where(e => string.IsNullOrWhiteSpace(input.ValueFilter)
                                                || (input.TypeFilter == 1 ? e.UserName.ToLower().Contains(input.ValueFilter.ToLower())
                                                    : (input.TypeFilter == 2 ? e.EmpName.ToLower().Contains(input.ValueFilter.ToLower())
                                                        : (input.TypeFilter == 3 ? e.Email.ToLower().Contains(input.ValueFilter.ToLower())
                                                            : e.PhoneNumber.ToLower().Contains(input.ValueFilter.ToLower())
                                                    )))
                                           )
                                    .Where(e => input.Gender > 0 ? input.Gender == e.Gender : true)
                                    .Where(e => e.IsDeleted == false)
                                    .Where(e => input.FromDate != null ? e.BirthDay >= input.FromDate : true)
                                    .Where(e => input.ToDate != null ? e.BirthDay < input.ToDate.Value.AddDays(1) : true)
                             orderby user.Id
                             select new GetAllUserDto
                             {
                                 Id = user.Id,
                                 BirthDay = user.BirthDay,
                                 PhoneNumber = user.PhoneNumber,
                                 EmpName = user.EmpName,
                                 UserName = user.UserName,
                                 Email = user.Email,
                                 Gender = user.Gender,
                             };
                var totalCount = result.Count();
                var pageAndFilterResult = result.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize);
                respon.Result = new PagedResultDto
                {
                    TotalCount = totalCount,
                    Result = pageAndFilterResult.ToList()
                };

            } catch (Exception ex)
            {
                respon.StatusCode = 500;
                respon.Message = ex.Message;
                respon.Result = new PagedResultDto
                {
                    TotalCount = 0,
                    Result = new List<GetAllUserDto>()
                };
            }
            return Task.FromResult(respon);
        }
        #endregion

        #region -- Thêm hoặc cập nhật thông tin user
        public async Task<ResponDto<bool>> CreateOrEditUser(CreateOrEditUserDto user)
        {
            var respon = new ResponDto<bool>();
            try
            {
                if (user.Id > 0) respon = await UpdateUser(user);
                else respon = await CreateUser(user);
            }
            catch (Exception ex)
            {
                respon.StatusCode = 500;
                respon.Message = ex.Message;
            }
            return respon;
        }

        // Cập nhật thông tin user
        private async Task<ResponDto<bool>> UpdateUser(CreateOrEditUserDto user)
        {
            var responUpdate = new ResponDto<bool>();
            var userUpdate = _unitOfWork.UserRepository.FirstOrDefault(e => e.Id == user.Id);
            if (userUpdate != null)
            {
                userUpdate.EmpName = user.EmpName;
                userUpdate.Email = user.Email;
                userUpdate.BirthDay = user.BirthDay;
                userUpdate.Gender = user.Gender;
                userUpdate.PhoneNumber = user.PhoneNumber;
                userUpdate.LastModifyUserId = user.CurrentUserId;
                userUpdate.LastModifyDate = DateTime.Now;
                userUpdate.Role = user.Role ?? 2;

                _unitOfWork.UserRepository.Update(userUpdate);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                responUpdate.StatusCode = 400;
                responUpdate.Message = $"Can not find user !";
                return responUpdate;
            }    
            return responUpdate;
        }
        // Thêm mới thông tin user
        private async Task<ResponDto<bool>> CreateUser(CreateOrEditUserDto user)
        {
            var responCreate = new ResponDto<bool>();
            var checkUser = _unitOfWork.UserRepository.FirstOrDefault(e => e.UserName == user.UserName);
            if (checkUser != null)
            {
                responCreate.StatusCode = 400;
                responCreate.Message = $"UserName is exist";
                return responCreate;
            }

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
                CreatorUserId = user.CurrentUserId,
                CreateDate = DateTime.Now,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = passwordSalt,
                Role = user.Role ?? 2,
            };
            _unitOfWork.UserRepository.Add(newUser);
            await _unitOfWork.SaveAsync();
            return responCreate;
        }
        // Tạo mã hóa thông tin user
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        #endregion

    }
}
