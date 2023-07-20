using Application.Interfaces;
using Application.IService;
using Domain.Dto.Users;
using Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
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
                });
                _unitOfWork.UserRepository.UpdateRange(users);
            }
            return Task.CompletedTask;
        }

        public async Task<List<User>> GetAll()
        {
            var result = await _unitOfWork.UserRepository.GetAllAsync();
            return result.ToList();
        }

        private async Task UpdateUser(CreateOrEditUserDto user)
        {
            var userUpdate = _unitOfWork.UserRepository.GetAll().Where(e => e.Id == user.Id).FirstOrDefault();
            if (userUpdate != null)
            {
                userUpdate.EmpName = user.EmpName;
                //userUpdate.UserName = user.UserName;
                //userUpdate.Password = user.Password;
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
            var checkUser = _unitOfWork.UserRepository.AnyAsync(e => e.UserName == user.UserName);
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
