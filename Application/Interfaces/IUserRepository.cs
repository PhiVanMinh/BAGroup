﻿using Application.Common.Interfaces;
using Application.Dto.Users;
using Domain.Master;

namespace Application.Interfaces
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public interface IUserRepository : IRepository<User>
    {
        Task<List<GetAllUserDto>> GetAllUsers(GetAllUserInput input);
    }
}
