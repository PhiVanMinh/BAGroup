using Application.Dto.Users;
using Application.Interfaces;
using Application.IService;
using Domain.Master;

namespace WebApi.Service
{
    public class UserService : IUserService
    {
        public IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateOrEditUser(CreateOrEditUserDto user)
        {
            if (user.Id > 0) await UpdateUser(user);
            else await CreateUser(user);
            return;
        }

        public Task DeleteUser(DeletedUserInput input)
        {
            var users = _unitOfWork.UserRepository.GetAll().Where(e => input.ListId.Contains(e.Id)).ToList();
            if (users.Count() > 0)
            {
                users.ForEach(e =>
                {
                    e.IsDeleted = true;
                    e.DeletedUserId = input.CurrentUserId;
                    e.LastModifyDate = DateTime.Now;
                });
                _unitOfWork.UserRepository.UpdateRange(users);
                _unitOfWork.SaveAsync();
            }
            return Task.CompletedTask;
        }

        public Task<PagedResultDto> GetAll(GetAllUserInput input)
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
            return Task.FromResult(
                    new PagedResultDto
                    {
                        TotalCount = totalCount,
                        Result = pageAndFilterResult.ToList()
                    }
                );
        }

        private async Task UpdateUser(CreateOrEditUserDto user)
        {
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

                _unitOfWork.UserRepository.Update(userUpdate);
                await _unitOfWork.SaveAsync();
            }
            else throw new Exception("Can not find user !");
            return;
        }

        private async Task CreateUser(CreateOrEditUserDto user)
        {
            var checkUser = _unitOfWork.UserRepository.FirstOrDefault(e => e.UserName == user.UserName);
            if (checkUser != null)
            {
                throw new Exception("UserName is exist");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

            var newUser = new User
            {
                EmpName = user.EmpName,
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                BirthDay = user.BirthDay,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                CreatorUserId = user.CurrentUserId,
                CreateDate = DateTime.Now,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };
            _unitOfWork.UserRepository.Add(newUser);
            await _unitOfWork.SaveAsync();
            return;
        }

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
