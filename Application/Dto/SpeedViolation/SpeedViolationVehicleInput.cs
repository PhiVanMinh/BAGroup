using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    /// <summary>Điều kiện lọc các xe quá tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/6/2023   created
    /// </Modified>
    public class SpeedViolationVehicleInput
    {
        /// <summary>Danh sách mã phương tiện</summary>
        public List<long> ListVhcId { get; set; }

        /// <summary>Từ ngày</summary>
        public DateTime? FromDate { get; set; }

        /// <summary>Đến ngày</summary>
        public DateTime? ToDate { get; set; }

        /// <summary>Số trang</summary>
        public int Page { get; set; }

        /// <summary>Kích thước trang</summary>
        public int PageSize { get; set; }
    }
}
