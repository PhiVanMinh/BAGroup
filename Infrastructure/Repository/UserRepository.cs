using Application.Interfaces;
using Domain.Master;
using Infrastructure.Persistence;
using Infrastructure.Common;
using Infrastructure.Contexts;
using Application.Dto.Users;
using Dapper;
using Application.Enum;
using System.Reflection;

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
            // Store procedure
            //var query = @"EXEC sp_get_all_users @sp_page = @Page, @sp_pageSize = @PageSize ,
            //            @sp_userName = @UserName,  @sp_fullName = @FullName, @sp_email = @Email, @sp_phoneNumber = @PhoneNumber, 
            //            @sp_gender = @Gender, @sp_fromDate = @FromDate, @sp_toDate = @ToDate";

            var query = @"
                        SELECT UserId, UserName, BirthDay, EmpName, PhoneNumber, Email, Gender, UserType 
		                FROM Users WITH(NOLOCK)
		                WHERE 
		                IsDeleted = 0
		                and (@Gender is null or @Gender = Gender)
		                and (@FromDate is null or CreateDate >= @FromDate)
		                and (@ToDate is null or CreateDate < DATEADD(day, 1, @ToDate))
		                and (@UserName is null or UserName LIKE  '%'+ @UserName +'%')
		                and (@FullName is null or EmpName LIKE  '%'+ @FullName +'%')
		                and (@Email is null or Email LIKE  '%'+ @Email +'%')
		                and (@PhoneNumber is null or PhoneNumber LIKE  '%'+ @PhoneNumber +'%')
		                ORDER BY CreateDate";

            using (var connection = _dapperContext.CreateConnection("Default"))
            {
                var userList = await connection.QueryAsync<GetAllUserDto>(query, new
                {
                    input.Page,
                    input.PageSize,
                    Gender = input.Gender > 0 ? input.Gender : null,
                    input.FromDate,
                    input.ToDate,
                    UserName = !string.IsNullOrWhiteSpace(input.UserName) ? input.UserName : null,
                    FullName = !string.IsNullOrWhiteSpace(input.FullName) ? input.FullName : null,
                    Email = !string.IsNullOrWhiteSpace(input.Email) ? input.Email : null,
                    PhoneNumber = !string.IsNullOrWhiteSpace(input.PhoneNumber) ? input.PhoneNumber : null,
                });
                return userList.ToList();
            }
        }
    }

}
