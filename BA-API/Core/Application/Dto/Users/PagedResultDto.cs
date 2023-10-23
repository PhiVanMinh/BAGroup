
using Application.Dto.SpeedViolation;

namespace Application.Dto.Users
{
    /// <summary>Thông tin phân trang</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class PagedResultDto<T>
    {
        /// <summary>Tổng số bản ghi</summary>
        public int TotalCount { get; set; }

        /// <summary>Danh sách dữ liệu</summary>
        public List<T> Result { get; set; }
    }
}
