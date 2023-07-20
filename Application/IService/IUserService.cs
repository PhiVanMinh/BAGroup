using Domain.Dto.Users;
using Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IUserService
    {
        public Task<List<User>> GetAll();
        public Task CreateOrEditUser (CreateOrEditUserDto user);
        public Task DeleteUser (DeletedUserInput input);
    }
}
