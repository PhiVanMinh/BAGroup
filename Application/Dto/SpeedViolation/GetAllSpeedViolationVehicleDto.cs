using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    public class GetAllSpeedViolationVehicleDto
    {
        public long? VehicleID { get; set; }
        public string? PrivateCode { get; set; }
        public string? TransportType { get; set; }
        public int? SpeedVioLevel1 { get; set; }
        public int? SpeedVioLevel2 { get; set; }
        public int? SpeedVioLevel3 { get; set; }
        public int? SpeedVioLevel4 { get; set; }
        public int? TotalSpeedVio { get; set; }
        public float? RatioSpeedVio { get; set; }
        public float? TotalKmVio { get; set; }
        public float? TotalKm { get; set; }
        public float? RatioKmVio { get; set; }
        public int? TotalTimeVio { get; set; }
        public int? TotalTime { get; set; }
        public float? RatioTimeVio { get; set; }
    }
}
