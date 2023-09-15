using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleInformation.Models
{
    /// <summary>Bảng tổng hợp hoạt động</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class Report_ActivitySummaries
    {

        /// <summary>Mã phương tiện</summary>
        public long FK_VehicleID { get; set; }

        /// <summary>Mã đơn vị vận tải</summary>
        public int FK_CompanyID { get; set; }

        /// <summary>Thời gian hoạt động</summary>
        public int? ActivityTime { get; set; }

        /// <summary>Tổng số km đã chạy</summary>
        public float? TotalKmGps { get; set; }

        /// <summary>Ngày tổng hợp</summary>
        public DateTime FK_Date { get; set; }
    }
}
