

namespace Application.Dto.Users
{
    /// <summary>Thông tin tìm kiếm</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// minhpv    28/8/2023   modified
    /// </Modified>
    public class GetAllUserInput
    {
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

        /// <summary>Tài khoản</summary>
        public string? UserName { get; set; }

        /// <summary>Tên người dùng</summary>
        public string? FullName { get; set; }

        /// <summary>Email</summary>
        public string? Email { get; set; }

        /// <summary>Số điện thoại</summary>
        public string? PhoneNumber { get; set; }
    }
}
