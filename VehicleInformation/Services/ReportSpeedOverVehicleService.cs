using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ReportSpeedOver.API.Protos;
using Services.Common.Core.Models;
using System;
using System.Threading.Tasks;
using VehicleInformation.Interfaces.IService;

using ReportDataService = ReportSpeedOver.API.Protos.ReportDataService;

namespace ReportSpeedOver.API.Services
{
    public class ReportSpeedOverVehicleService : ReportDataService.ReportDataServiceBase
    {
        private readonly IActivitySummariesService _activitySummaries;
        private readonly ISpeedOversService _speedOvers;
        private readonly ITransportTypesService _transportTypes;
        private readonly IVehiclesService _vehicle;
        private readonly IVehicleTransportTypesService _vhcTransportTypes;
        private readonly ILogger<ReportSpeedOverVehicleService> _logger;
        private readonly IMapper _mapper;

        public ReportSpeedOverVehicleService(
            IActivitySummariesService activitySummaries,
            ISpeedOversService speedOvers,
            ITransportTypesService transportTypes,
            IVehiclesService vehicle,
            IVehicleTransportTypesService vhcTransportTypes,
            ILogger<ReportSpeedOverVehicleService> logger,
            IMapper mapper
        )
        {
            _activitySummaries = activitySummaries;
            _speedOvers = speedOvers;
            _transportTypes = transportTypes;
            _vehicle = vehicle;
            _vhcTransportTypes = vhcTransportTypes;
            _logger = logger;
            _mapper = mapper;
        }

        public async override Task<ActivitySummaries> GetActivitySummaries(GetById request, ServerCallContext context)
        {
            ActivitySummaries response = new ActivitySummaries();
            try
            {
                var result = await _activitySummaries.GetAllByCompany(request.Id);
                foreach (Report_ActivitySummaries value in result)
                {
                    response.Items.Add(_mapper.Map<ActivitySummary>(value));
                }
            } catch (Exception ex)
            {
                _logger.LogInformation($"ReportSpeedOverVehicleService_GetActivitySummaries_{request.Id}_{ex.Message}");
            }
            return response;
        }

        public async override Task<SpeedOvers> GetSpeedOver(GetSpeedOverRequest request, ServerCallContext context)
        {
            SpeedOvers response = new SpeedOvers();
            try
            {
                var result = await _speedOvers.GetAllSpeedOversByDate(request.FromDate.ToDateTime(), request.ToDate.ToDateTime());
                foreach (BGT_SpeedOvers value in result)
                {
                    response.Items.Add(_mapper.Map<SpeedOver>(value));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ReportSpeedOverVehicleService_GetActivitySummaries_{request.FromDate.ToDateTime()}_{request.ToDate.ToDateTime()}_{ex.Message}");
            }
            return response;
        }

        public async override Task<TranportTypes> GetTransportTypes(Empty request, ServerCallContext context)
        {
            TranportTypes response = new TranportTypes();
            try
            {
                var result = await _transportTypes.GetAll();
                foreach (BGT_TranportTypes value in result)
                {
                    var item = new TranportType
                    {
                        DisplayName = value.DisplayName,
                        IsActivated = value.IsActivated,
                        PKTransportTypeID = value.PK_TransportTypeID
                    };
                    //response.Items.Add(_mapper.Map<TranportType>(value));
                    response.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ReportSpeedOverVehicleService_GetActivitySummaries_{ex.Message}");
            }
            return response;
        }

        public async override Task<Vehicles> GetVehicleInfo(GetById request, ServerCallContext context)
        {
            Vehicles response = new Vehicles();
            try
            {
                var result = await _vehicle.GetAllByCompany(request.Id);
                foreach (Vehicle_Vehicles value in result)
                {
                    response.Items.Add(_mapper.Map<Vehicle>(value));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ReportSpeedOverVehicleService_GetVehicleInfo_{request.Id}_{ex.Message}");
            }
            return response;
        }

        public async override Task<VehicleTransportTypes> GetVehicleTransportType(Empty request, ServerCallContext context)
        {
            VehicleTransportTypes response = new VehicleTransportTypes();
            try
            {
                var result = await _vhcTransportTypes.GetAll();
                foreach (BGT_VehicleTransportTypes value in result)
                {
                    response.Items.Add(_mapper.Map<VehicleTransportType>(value));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ReportSpeedOverVehicleService_GetVehicleTransportType_{ex.Message}");
            }
            return response;
        }
    }
}
