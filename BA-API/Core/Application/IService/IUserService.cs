
using Application.Dto.Common;
using Application.Dto.Users;

namespace Application.IService
{
    public interface IUserService
    {
        public Task<PagedResultDto<GetAllUserDto>> GetAll(GetAllUserInput input);
        public Task<string> CreateOrEditUser (CreateOrEditUserDto user);
        public Task<string> DeleteUser (DeletedUserInput input);
        public Task<List<GetAllUserDto>> GetDataToExportExcel(GetAllUserInput input);
    }
}
