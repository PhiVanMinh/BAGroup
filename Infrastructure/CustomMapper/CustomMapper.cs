using Application.Dto.Users;
using AutoMapper;
using Domain.Master;

namespace Infrastructure.CustomMapper
{
    public class CustomMapper: Profile
    {

        public CustomMapper ()
        {
            CreateMap<User, UserLoginInfo>();
        }
    }
}
