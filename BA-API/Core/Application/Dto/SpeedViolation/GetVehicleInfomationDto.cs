using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    /// <summary>Thông tin xe</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class GetVehicleInfomationDto
    {
        /// <summary>Mã phương tiện</summary>
        public long? VehicleID { get; set; }

        /// <summary>Mã công ty</summary>
        public long? CompanyID { get; set; }

        /// <summary>Biển kiểm soát</summary>
        public string? VehiclePlate { get; set; }

        /// <summary>Số hiệu xe</summary>
        public string? PrivateCode { get; set; }

        /// <summary>Loại hình</summary>
        public string? TransportType { get; set; }
    }
}
