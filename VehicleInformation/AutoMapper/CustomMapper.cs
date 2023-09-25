using AutoMapper;
using ReportSpeedOver.API.Protos;
using Services.Common.Core.Models;
//using VehicleInformation.Models;

namespace ReportSpeedOver.API.AutoMapper
{
    /// <summary>Ánh xạ dữ liệu giữa 2 thực thể</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class CustomMapper : Profile
    {

        public CustomMapper()
        {
            CreateMap<Report_ActivitySummaries, ActivitySummary>().ReverseMap();
            CreateMap<BGT_SpeedOvers, SpeedOver>().ReverseMap();
            CreateMap<BGT_TranportTypes, TranportType>().ReverseMap();
            CreateMap<Vehicle_Vehicles, Vehicle>().ReverseMap();
            CreateMap<BGT_VehicleTransportTypes, VehicleTransportType>().ReverseMap();

        }
    }
}
