using Application.Common.Interfaces;
using Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthRepository 
    {
        Task<User> Login(string username, string password);
    }
}
