using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ReportDataGrpcService.Interfaces.IRepository;
using Services.Common.Core.Models;

namespace ReportDataGrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly IVehicleTransportTypesRepository _vehicleTransportTypesRepository;
        private readonly ISpeedOversRepository _speedOversRepository;
        private readonly IActivitySummariesRepository _activitySummariesRepository;
        private readonly ITransportTypesRepository _transportTypesRepository;
        private readonly IVehiclesRepository _vehiclesRepository;

        //private readonly IActivitySummariesService _activitySummaries;
        //private readonly ISpeedOversService _speedOvers;
        //private readonly ITransportTypesService _transportTypes;
        //private readonly IVehiclesService _vehicle;
        //private readonly IVehicleTransportTypesService _vhcTransportTypes;
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(
            IVehicleTransportTypesRepository vehicleTransportTypesRepository,
            ISpeedOversRepository speedOversRepository,
            IActivitySummariesRepository activitySummariesRepository,
            IVehiclesRepository vehiclesRepository,
            ITransportTypesRepository transportTypesRepository,

            //IActivitySummariesService activitySummaries,
            //ISpeedOversService speedOvers,
            //ITransportTypesService transportTypes,
            //IVehiclesService vehicle,
            //IVehicleTransportTypesService vhcTransportTypes,
            ILogger<GreeterService> logger
        )
        {
            _vehicleTransportTypesRepository = vehicleTransportTypesRepository;
            _speedOversRepository = speedOversRepository;
            _activitySummariesRepository = activitySummariesRepository;
            _transportTypesRepository = transportTypesRepository;
            _vehiclesRepository = vehiclesRepository;

            //_activitySummaries = activitySummaries;
            //_speedOvers = speedOvers;
            //_transportTypes = transportTypes;
            //_vehicle = vehicle;
            //_vhcTransportTypes = vhcTransportTypes;
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public async override Task<ActivitySummaries> GetActivitySummaries(GetById request, ServerCallContext context)
        {
            ActivitySummaries response = new ActivitySummaries();
            try
            {
                var result = await GetAllActivitySummariesByCompany(request.Id);
                foreach (Report_ActivitySummaries value in result)
                {
                    var item = new ActivitySummary
                    {
                        FKVehicleID = value.FK_VehicleID,
                        FKCompanyID = value.FK_CompanyID,
                        ActivityTime = value.ActivityTime ?? 0,
                        FKDate = Timestamp.FromDateTime(value.FK_Date .ToUniversalTime()),
                        TotalKmGps = value.TotalKmGps ?? 0f
                    };
                    response.Items.Add(item);
                }
            }
            catch (Exception ex)
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
                var result = await GetAllSpeedOversByDate(DateTime.Parse(request.FromDate), DateTime.Parse(request.ToDate));
                foreach (BGT_SpeedOvers value in result)
                {
                    var item = new SpeedOver
                    {
                        FKVehicleID = value.FK_VehicleID,
                        FKCompanyID = value.FK_CompanyID,
                        VelocityAllow = value.VelocityAllow ?? 0,
                        VelocityGps = value.VelocityGps ?? 0,
                        CreatedDate = Timestamp.FromDateTime((value.CreatedDate ?? DateTime.Now).ToUniversalTime()),
                        StartKm = value.StartKm ?? 0,
                        EndKm = value.EndKm ?? 0,
                        StartTime = Timestamp.FromDateTime((value.StartTime?? DateTime.Now).ToUniversalTime()),
                        EndTime = Timestamp.FromDateTime((value.EndTime ?? DateTime.Now).ToUniversalTime()),
                    };
                    response.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ReportSpeedOverVehicleService_GetActivitySummaries_{request.FromDate}_{request.ToDate}_{ex.Message}");
            }
            return response;
        }

        //public async override Task<TranportTypes> GetTransportTypes(Empty request, ServerCallContext context)
        //{
        //    TranportTypes response = new TranportTypes();
        //    try
        //    {
        //        var result = await _transportTypes.GetAll();
        //        foreach (BGT_TranportTypes value in result)
        //        {
        //            var item = new TranportType
        //            {
        //                DisplayName = value.DisplayName,
        //                IsActivated = value.IsActivated,
        //                PKTransportTypeID = value.PK_TransportTypeID
        //            };
        //            //response.Items.Add(_mapper.Map<TranportType>(value));
        //            response.Items.Add(item);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation($"ReportSpeedOverVehicleService_GetActivitySummaries_{ex.Message}");
        //    }
        //    return response;
        //}

        //public async override Task<Vehicles> GetVehicleInfo(GetById request, ServerCallContext context)
        //{
        //    Vehicles response = new Vehicles();
        //    try
        //    {
        //        var result = await _vehicle.GetAllByCompany(request.Id);
        //        //foreach (Vehicle_Vehicles value in result)
        //        //{
        //        //    response.Items.Add(_mapper.Map<Vehicle>(value));
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation($"ReportSpeedOverVehicleService_GetVehicleInfo_{request.Id}_{ex.Message}");
        //    }
        //    return response;
        //}

        public async override Task<VehicleTransportTypes> GetVehicleTransportType(Empty request, ServerCallContext context)
        {
            VehicleTransportTypes response = new VehicleTransportTypes();
            try
            {
                var result = await GetAllVehicleTransportTypes();
                foreach (BGT_VehicleTransportTypes value in result)
                {
                    var item = new VehicleTransportType
                    {
                        FKVehicleID = value.FK_VehicleID,
                        FKCompanyID = value.FK_CompanyID,
                        FKTransportTypeID = value.FK_TransportTypeID,
                        IsDeleted = value.IsDeleted ?? false,
                    };
                    response.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ReportSpeedOverVehicleService_GetVehicleTransportType_{ex.Message}");
            }
            return response;
        }

        /// <summary>Lấy thông tin tổng hợp theo đơn vị vẫn tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin tổng hợp</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        private Task<List<Report_ActivitySummaries>> GetAllActivitySummariesByCompany(int input)
        {
            var result = new List<Report_ActivitySummaries>();
            try
            {
                //var cacheKey = $"ActivitySummariesService_GetAllByCompany_Report_ActivitySummaries";
                //result = await _cacheHelper.GetDataFromCache<Report_ActivitySummaries>(cacheKey, 0, 0);
                //if (result.Count() == 0)
                //{
                    var summaries = _activitySummariesRepository.GetAllByColumnId("FK_CompanyID", input, false);
                    result = summaries.ToList();
                //    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                //}
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetActivitySummaries_{ex.Message}_{input}");
            }
            return Task.FromResult(result);
        }

        /// <summary>Thông tin loại hình vận tải của phương tiện</summary>
        /// <returns>Các thông tin loại hình vận tải của phương tiện</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        private Task<List<BGT_VehicleTransportTypes>> GetAllVehicleTransportTypes()
        {
            var result = new List<BGT_VehicleTransportTypes>();
            try
            {
                //var cacheKey = $"VehicleTransportTypesService_GetAll_BGT_VehicleTransportTypes";
                //result = await _cacheHelper.GetDataFromCache<BGT_VehicleTransportTypes>(cacheKey, 0, 0);
                //if (result.Count() == 0)
                //{
                    var respon = _vehicleTransportTypesRepository.GetAll();
                    result = respon.ToList();
                    //_cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                //}
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicleTransportTypes_{ex.Message}");
            }
            return Task.FromResult(result);
        }

        /// <summary>Thông tin vi phạm tốc độ theo ngày</summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Các thông tin vi phạm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        private Task<List<BGT_SpeedOvers>> GetAllSpeedOversByDate(DateTime fromDate, DateTime toDate)
        {
            var result = new List<BGT_SpeedOvers>();
            try
            {
                //var cacheKey = $"SpeedOversService_GetAllSpeedOversByDate_BGT_SpeedOvers";
                //result = await _cacheHelper.GetDataFromCache<BGT_SpeedOvers>(cacheKey, 0, 0);
                //if (result.Count() == 0)
                //{
                    var speedOvers = _speedOversRepository.GetAllByDate("StartTime", fromDate, toDate, false);
                    result = speedOvers.ToList();
                //    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                //}
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetSpeedOvers_{ex.Message}_{fromDate}_{toDate}");
            }
            return Task.FromResult(result);
        }
    }
}