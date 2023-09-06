

namespace Application.Dto.SpeedViolation
{
    /// <summary>Thông tin xe vi phạm quá tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/6/2023   created
    /// </Modified>
    public class GetAllSpeedViolationVehicleDto
    {
        /// <summary>Mã phương tiện</summary>
        public long? VehicleID { get; set; }

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
        public float? TotalKmVio { get; set; }

        /// <summary>Tổng số km đã đi được</summary>
        public float? TotalKm { get; set; }

        /// <summary>Tỷ lệ km vi phạm / tổng km đã đi</summary>
        public float? RatioKmVio { get; set; }

        /// <summary>Tổng số thời gian vi phạm</summary>
        public int? TotalTimeVio { get; set; }

        /// <summary>Tổng số thời gian đã đi</summary>
        public int? TotalTime { get; set; }

        /// <summary>Tỷ lệ thời gian vi phạm / tổng thời gian đã đi</summary>
        public float? RatioTimeVio { get; set; }
    }
}
