

using Application.Dto.Users;

namespace Application.IService
{
    public interface IUserService
    {
        public Task<PagedResultDto> GetAll(GetAllUserInput input);
        public Task CreateOrEditUser (CreateOrEditUserDto user);
        public Task DeleteUser (DeletedUserInput input);
    }
}
