using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleInformation.Models
{
    /// <summary>Thông tin vi phạm tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class BGT_SpeedOvers
    {
        /// <summary>Mã phương tiện</summary>
        public long FK_VehicleID { get; set; }

        /// <summary>Mã đơn vị vận tải</summary>
        public int FK_CompanyID { get; set; }

        /// <summary>Vận tốc cho phép</summary>
        public int? VelocityAllow { get; set; }

        /// <summary>Vận tốc thực tế</summary>
        public int? VelocityGps { get; set; }

        /// <summary>Km bắt đầu vi phạm</summary>
        public float? StartKm { get; set; }

        /// <summary>Km kết thúc vi phạm</summary>
        public float? EndKm { get; set; }

        /// <summary>Thời gian bắt đầu vi phạm</summary>
        public DateTime? StartTime { get; set; }

        /// <summary>Thời gian kết thúc vi phạm</summary>
        public DateTime? EndTime { get; set; }

        /// <summary>Ngày tạo</summary>
        public DateTime? CreatedDate { get; set; }

    }
}
