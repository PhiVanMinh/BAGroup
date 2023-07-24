using AutoMapper;
using Domain.Dto.Users;
using Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CustomMapper
{
    public class CustomMapper: Profile
    {

        public CustomMapper ()
        {
            CreateMap<User, UserLogInInfo>();
        }
    }
}
