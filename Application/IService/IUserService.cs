
using Application.Dto.Common;
using Application.Dto.Users;

namespace Application.IService
{
    public interface IUserService
    {
        public Task<ResponDto<PagedResultDto>> GetAll(GetAllUserInput input);
        public Task<ResponDto<bool>> CreateOrEditUser (CreateOrEditUserDto user);
        public Task<ResponDto<bool>> DeleteUser (DeletedUserInput input);
    }
}
