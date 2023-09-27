using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Services.Common.Core.Entity;
using Services.Common.Core.Models;

namespace ReportDataGrpcService.AutoMapper
{

    /// <summary>Ánh xạ dữ liệu giữa 2 thực thể</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class Mapper : Profile
    {

        public Mapper()
        {

            CreateMap<Report_ActivitySummaries, ActivitySummary>()
                .ForMember(d => d.FKVehicleID, e => e.MapFrom(o => o.FK_VehicleID))
                .ForMember(d => d.FKCompanyID, e => e.MapFrom(o => o.FK_CompanyID))
                .ForMember(d => d.FKDate, e => e.MapFrom(o => Timestamp.FromDateTime(o.FK_Date.ToUniversalTime())));

            CreateMap<BGT_SpeedOvers, SpeedOver>()
                .ForMember(d => d.FKVehicleID, e => e.MapFrom(o => o.FK_VehicleID))
                .ForMember(d => d.FKCompanyID, e => e.MapFrom(o => o.FK_CompanyID))
                .ForMember(d => d.EndTime, e => e.MapFrom(o => Timestamp.FromDateTime((o.EndTime ?? DateTime.Now).ToUniversalTime())))
                .ForMember(d => d.StartTime, e => e.MapFrom(o => Timestamp.FromDateTime((o.StartTime ?? DateTime.Now).ToUniversalTime())))
                .ForMember(d => d.CreatedDate, e => e.MapFrom(o => Timestamp.FromDateTime((o.CreatedDate ?? DateTime.Now).ToUniversalTime())));

            CreateMap<BGT_TranportTypes, TranportType>().ForMember(d => d.PKTransportTypeID, e => e.MapFrom(o => o.PK_TransportTypeID));

            CreateMap<Vehicle_Vehicles, Vehicle>()
                .ForMember(d => d.PKVehicleID, e => e.MapFrom(o => o.PK_VehicleID))
                .ForMember(d => d.FKCompanyID, e => e.MapFrom(o => o.FK_CompanyID));

            CreateMap<BGT_VehicleTransportTypes, VehicleTransportType>()
                .ForMember(d => d.FKVehicleID, e => e.MapFrom(o => o.FK_VehicleID))
                .ForMember(d => d.FKCompanyID, e => e.MapFrom(o => o.FK_CompanyID))
                .ForMember(d => d.FKTransportTypeID, e => e.MapFrom(o => o.FK_TransportTypeID));
        }
    }
}
