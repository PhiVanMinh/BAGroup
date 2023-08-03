
using Application.Enum;

namespace Application.Dto.Users
{
    public class GetAllUserInput
    {
        // Giá trị tìm kiếm
        public string? ValueFilter { get; set; }

        // Loại tìm kiếm 
        public FilterType? TypeFilter { get; set; }

        // Giới tính
        public byte? Gender { get; set; }

        // Ngày sinh từ ngày
        public DateTime? FromDate { get; set; }

        // Ngày sinh đến ngày
        public DateTime? ToDate { get; set; }

        // Số trang
        public int Page { get; set;}

        // Kích thước trang
        public int PageSize { get; set; }
    }
}
