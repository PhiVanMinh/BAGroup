
using Application.Enum;

namespace Application.Dto.Users
{
    /// <summary>Thông tin tìm kiếm</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class GetAllUserInput
    {
        /// <summary>Giá trị tìm kiếm</summary>
        public string? ValueFilter { get; set; }

        /// <summary>Loại tìm kiếm</summary>
        public FilterType? TypeFilter { get; set; }

        /// <summary>Giới tính</summary>
        public byte? Gender { get; set; }

        /// <summary>Từ ngày</summary>
        public DateTime? FromDate { get; set; }

        /// <summary>Đến ngày</summary>
        public DateTime? ToDate { get; set; }

        /// <summary>Số trang</summary>
        public int Page { get; set;}

        /// <summary>Kích thước trang</summary>
        public int PageSize { get; set; }
    }
}
