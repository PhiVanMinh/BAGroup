
namespace Application.Dto.Users
{
    public class PagedResultDto
    {
        // Tổng số bản ghi
        public int TotalCount { get; set; }

        // Danh sách bản ghi
        public List<GetAllUserDto> Result { get; set; }
    }
}
