
namespace Application.Dto.Users
{
    public class PagedResultDto
    {
        public int TotalCount { get; set; }
        public List<GetAllUserDto> Result { get; set; }
    }
}
