

namespace VehicleInformation.Models
{
    /// <summary>Thông tin xe</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class Vehicle_Vehicles
    {
        /// <summary>Mã phương tiện</summary>
        public long PK_VehicleID { get; set; }

        /// <summary>Mã đơn vị vận tải</summary>
        public int FK_CompanyID { get; set; }

        /// <summary>Kí hiệu xe</summary>
        public string PrivateCode { get; set; }

        /// <summary>Biển kiểm soát</summary>
        public string VehiclePlate { get; set; }

        /// <summary>Xoá bản ghi</summary>
        public bool? IsDeleted { get; set; }
    }
}
