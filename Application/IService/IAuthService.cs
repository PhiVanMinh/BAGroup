using Domain.Dto.Users;
using Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IAuthService
    {
        public Task<User> Login(UserForLoginDto userLogin);
    }
}
