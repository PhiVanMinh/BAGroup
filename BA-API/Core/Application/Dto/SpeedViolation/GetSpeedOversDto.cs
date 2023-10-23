using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    /// <summary>Thông tin vi phạm tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class GetSpeedOversDto
    {
        /// <summary>Mã phương tiện</summary>
        public long? VehicleID { get; set; }

        /// <summary>Số lần vi phẹm tốc độ 5-10 Km</summary>
        public int? SpeedVioLevel1 { get; set; }

        /// <summary>Số lần vi phẹm tốc độ 10-20 Km</summary>
        public int? SpeedVioLevel2 { get; set; }

        /// <summary>Số lần vi phẹm tốc độ 20-35 Km</summary>
        public int? SpeedVioLevel3 { get; set; }

        /// <summary>Số lần vi phẹm tốc độ trên 35 Km</summary>
        public int? SpeedVioLevel4 { get; set; }

        /// <summary>Tổng số lần vi tốc độ </summary>
        public int? TotalSpeedVio { get; set; }

        /// <summary>Tỷ lệ vi phạm tốc độ / 1000 km</summary>
        public float? RatioSpeedVio { get; set; }

        /// <summary>Tổng số km vi phạm tốc độ</summary>
        public float? TotalKmVio { get; set; }

        /// <summary>Tổng số thời gian vi phạm</summary>
        public int? TotalTimeVio { get; set; }
    }
}
