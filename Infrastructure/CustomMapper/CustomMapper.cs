using Application.Dto.Users;
using AutoMapper;
using Services.Common.Core.Entity;

namespace Infrastructure.CustomMapper
{
    /// <summary>Ánh xạ dữ liệu giữa 2 thực thể</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class CustomMapper: Profile
    {

        public CustomMapper ()
        {
            CreateMap<User, UserLoginInfo>();
        }
    }
}
