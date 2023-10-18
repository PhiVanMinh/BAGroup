using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    public class GetSpeedViolationToExportDto
    {
        /// <summary>Biển kiểm soát</summary>
        public string? VehiclePlate { get; set; }

        /// <summary>Số hiệu xe</summary>
        public string? PrivateCode { get; set; }

        /// <summary>Loại hình</summary>
        public string? TransportType { get; set; }

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
        public decimal? TotalKmVio { get; set; }

        /// <summary>Tổng số km đã đi được</summary>
        public decimal? TotalKm { get; set; }

        /// <summary>Tỷ lệ km vi phạm / tổng km đã đi</summary>
        public decimal? RatioKmVio { get; set; }

        /// <summary>Tổng số thời gian vi phạm</summary>
        public string? TotalTimeVio { get; set; }

        /// <summary>Tổng số thời gian đã đi</summary>
        public string? TotalTime { get; set; }

        /// <summary>Tỷ lệ thời gian vi phạm / tổng thời gian đã đi</summary>
        public decimal? RatioTimeVio { get; set; }
    }
}
