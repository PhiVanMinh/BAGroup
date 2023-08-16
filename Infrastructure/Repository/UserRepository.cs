using Application.Interfaces;
using Domain.Master;
using Infrastructure.Persistence;
using Infrastructure.Common;
using Infrastructure.Contexts;
using Application.Dto.Users;
using Dapper;

namespace Infrastructure.Repository
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly DapperContext _dapperContext;

        public UserRepository(ApplicationDBContext _dbContext, DapperContext dapperContext) : base(_dbContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<List<GetAllUserDto>> GetAllUsers(GetAllUserInput input)
        {
            var query = "SELECT * FROM Users";
            using (var connection = _dapperContext.CreateConnection())
            {
                var userList = await connection.QueryAsync<GetAllUserDto>(query);
                return userList.ToList();
            }
        }
    }

}
