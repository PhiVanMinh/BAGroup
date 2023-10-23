using Application.Dto.Users;
using AutoMapper;
using ReportDataGrpcService;
using Services.Common.Core.Entity;
using Services.Common.Core.Models;

namespace Infrastructure.CustomMapper
{
    /// <summary>Ánh xạ dữ liệu giữa 2 thực thể</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class CustomMapper: Profile
    {

        public CustomMapper ()
        {
            CreateMap<User, UserLoginInfo>();

            CreateMap<ActivitySummary, Report_ActivitySummaries>()
                .ForMember(d => d.FK_VehicleID, e => e.MapFrom(o => o.FKVehicleID))
                .ForMember(d => d.FK_CompanyID, e => e.MapFrom(o => o.FKCompanyID))
                .ForMember(d => d.FK_Date, e => e.MapFrom(o => o.FKDate.ToDateTime()));

            CreateMap<SpeedOver, BGT_SpeedOvers>()
                .ForMember(d => d.FK_VehicleID, e => e.MapFrom(o => o.FKVehicleID))
                .ForMember(d => d.FK_CompanyID, e => e.MapFrom(o => o.FKCompanyID))
                .ForMember(d => d.EndTime, e => e.MapFrom(o => o.EndTime.ToDateTime()))
                .ForMember(d => d.StartTime, e => e.MapFrom(o => o.StartTime.ToDateTime()))
                .ForMember(d => d.CreatedDate, e => e.MapFrom(o => o.CreatedDate.ToDateTime()));

            CreateMap<TranportType, BGT_TranportTypes>().ForMember(d => d.PK_TransportTypeID, e => e.MapFrom(o => o.PKTransportTypeID));

            CreateMap<Vehicle, Vehicle_Vehicles>()
                .ForMember(d => d.PK_VehicleID, e => e.MapFrom(o => o.PKVehicleID))
                .ForMember(d => d.FK_CompanyID, e => e.MapFrom(o => o.FKCompanyID));

            CreateMap<VehicleTransportType, BGT_VehicleTransportTypes>()
                .ForMember(d => d.FK_VehicleID, e => e.MapFrom(o => o.FKVehicleID))
                .ForMember(d => d.FK_CompanyID, e => e.MapFrom(o => o.FKCompanyID))
                .ForMember(d => d.FK_TransportTypeID, e => e.MapFrom(o => o.FKTransportTypeID));
        }
    }
}
