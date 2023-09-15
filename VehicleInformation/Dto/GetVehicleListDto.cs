namespace VehicleInformation.Dto
{
    /// <summary>Thông tin xe theo đơn vị vận tải</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/6/2023   created
    /// </Modified>
    public class GetVehicleListDto
    {
        /// <summary>Mã phương tiện</summary>
        public int VehicleID { get; set; }

        /// <summary>Số hiệu xe</summary>
        public string? PrivateCode { get; set; }
    }
}
