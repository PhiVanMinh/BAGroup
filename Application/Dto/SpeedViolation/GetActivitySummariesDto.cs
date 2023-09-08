using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    /// <summary>Thông tin tổng hợp</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class GetActivitySummariesDto
    {
        /// <summary>Mã phương tiện</summary>
        public long VehicleID { get; set; }

        /// <summary>Mã công ty</summary>
        public int CompanyId { get; set; }

        /// <summary>Tổng số km đã đi được</summary>
        public float? TotalKm { get; set; }

        /// <summary>Tổng số thời gian đã đi</summary>
        public int? TotalTime { get; set; }
    }
}
