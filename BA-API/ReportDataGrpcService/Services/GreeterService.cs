using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using ReportDataGrpcService.Interfaces.IHelper;
using ReportDataGrpcService.Interfaces.IRepository;
using Services.Common.Core.Models;
using StackExchange.Redis;

namespace ReportDataGrpcService.Services
{
    /// <summary>Các hàm truyền dữ liệu qua gRPC</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/27/2023   created
    /// </Modified>
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly IVehicleTransportTypesRepository _vehicleTransportTypesRepository;
        private readonly ISpeedOversRepository _speedOversRepository;
        private readonly IActivitySummariesRepository _activitySummariesRepository;
        private readonly ITransportTypesRepository _transportTypesRepository;
        private readonly IVehiclesRepository _vehiclesRepository;
        private readonly IConfiguration _configuration;
        private readonly ICacheHelper _cacheHelper;
        private readonly IDatabase _cache;

        private readonly ILogger<GreeterService> _logger;
        public GreeterService(
            IVehicleTransportTypesRepository vehicleTransportTypesRepository,
            ISpeedOversRepository speedOversRepository,
            IActivitySummariesRepository activitySummariesRepository,
            IVehiclesRepository vehiclesRepository,
            ITransportTypesRepository transportTypesRepository,

            ILogger<GreeterService> logger,
            IConfiguration configuration,
            ICacheHelper cacheHelper
        )
        {
            _vehicleTransportTypesRepository = vehicleTransportTypesRepository;
            _speedOversRepository = speedOversRepository;
            _activitySummariesRepository = activitySummariesRepository;
            _transportTypesRepository = transportTypesRepository;
            _vehiclesRepository = vehiclesRepository;

            _logger = logger;
            _configuration = configuration;
            _cacheHelper = cacheHelper;
            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Hàm truyền DL tổng hợp</summary>
        /// <param name="request">The request received from the client.</param>
        /// <param name="context">The context of the server-side call handler being invoked.</param>
        /// <returns>The response to send back to the client (wrapped by a task).</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/27/2023   created
        /// </Modified>
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
                _logger.LogInformation($"GreeterService_GetActivitySummaries_{request.Id}_{ex.Message}");
            }
            return response;
        }

        /// <summary>Hàm chuyền DL vi phạm tốc độ</summary>
        /// <param name="request">The request received from the client.</param>
        /// <param name="context">The context of the server-side call handler being invoked.</param>
        /// <returns>The response to send back to the client (wrapped by a task).</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/27/2023   created
        /// </Modified>
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
                _logger.LogInformation($"GreeterService_GetSpeedOver_{request.FromDate}_{request.ToDate}_{ex.Message}");
            }
            return response;
        }

        /// <summary>Hàm truyền DL loại hình vận tải</summary>
        /// <param name="request">The request received from the client.</param>
        /// <param name="context">The context of the server-side call handler being invoked.</param>
        /// <returns>The response to send back to the client (wrapped by a task).</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/27/2023   created
        /// </Modified>
        public async override Task<TranportTypes> GetTransportTypes(Empty request, ServerCallContext context)
        {
            TranportTypes response = new TranportTypes();
            try
            {
                var result = await GetAllTranportTypes();
                foreach (BGT_TranportTypes value in result)
                {
                    var item = new TranportType
                    {
                        DisplayName = value.DisplayName,
                        IsActivated = value.IsActivated,
                        PKTransportTypeID = value.PK_TransportTypeID
                    };
                    response.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GreeterService_GetTransportTypes_{ex.Message}");
            }
            return response;
        }

        /// <summary>Hàm truyền DL thông tin xe</summary>
        /// <param name="request">The request received from the client.</param>
        /// <param name="context">The context of the server-side call handler being invoked.</param>
        /// <returns>The response to send back to the client (wrapped by a task).</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/27/2023   created
        /// </Modified>
        public async override Task<Vehicles> GetVehicleInfo(GetById request, ServerCallContext context)
        {
            Vehicles response = new Vehicles();
            try
            {
                var result = await GetAllVehicleByCompany(request.Id);
                foreach (Vehicle_Vehicles value in result)
                {
                    var item = new Vehicle
                    {
                        PKVehicleID = value.PK_VehicleID,
                        FKCompanyID = value.FK_CompanyID,
                        PrivateCode = value.PrivateCode,
                        VehiclePlate = value.VehiclePlate,
                        IsDeleted = value.IsDeleted ?? false
                    };
                    response.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GreeterService_GetVehicleInfo_{request.Id}_{ex.Message}");
            }
            return response;
        }

        /// <summary>Hàm truyền DL thông tin vận tải của xe</summary>
        /// <param name="request">The request received from the client.</param>
        /// <param name="context">The context of the server-side call handler being invoked.</param>
        /// <returns>The response to send back to the client (wrapped by a task).</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/27/2023   created
        /// </Modified>
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
                _logger.LogInformation($"GreeterService_GetVehicleTransportType_{ex.Message}");
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
        private async Task<List<Report_ActivitySummaries>> GetAllActivitySummariesByCompany(int input)
        {
            var result = new List<Report_ActivitySummaries>();
            try
            {
                var cacheKey = $"GreeterService_GetAllActivitySummariesByCompany_{input}";
                result = await _cacheHelper.GetDataFromCache<Report_ActivitySummaries>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var summaries = _activitySummariesRepository.GetAllByColumnId("FK_CompanyID", input, false);
                    result = summaries.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GreeterService_GetAllActivitySummariesByCompany_{ex.Message}_{input}");
            }
            return result;
        }

        /// <summary>Thông tin loại hình vận tải của phương tiện</summary>
        /// <returns>Các thông tin loại hình vận tải của phương tiện</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        private async Task<List<BGT_VehicleTransportTypes>> GetAllVehicleTransportTypes()
        {
            var result = new List<BGT_VehicleTransportTypes>();
            try
            {
                var cacheKey = $"GreeterService_GetAllVehicleTransportTypes";
                result = await _cacheHelper.GetDataFromCache<BGT_VehicleTransportTypes>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var respon = _vehicleTransportTypesRepository.GetAll();
                    result = respon.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GreeterService_GetAllVehicleTransportTypes_{ex.Message}");
            }
            return result;
        }

        /// <summary>Thông tin vi phạm tốc độ theo ngày</summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Các thông tin vi phạm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        private async Task<List<BGT_SpeedOvers>> GetAllSpeedOversByDate(DateTime fromDate, DateTime toDate)
        {
            var result = new List<BGT_SpeedOvers>();
            try
            {
                var cacheKey = $"GreeterService_GetAllSpeedOversByDate_BGT_SpeedOvers_{fromDate}_{toDate}";
                result = await _cacheHelper.GetDataFromCache<BGT_SpeedOvers>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var speedOvers = _speedOversRepository.GetAllByDate("StartTime", fromDate, toDate, false);
                    result = speedOvers.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GreeterService_GetAllSpeedOversByDate_{ex.Message}_{fromDate}_{toDate}");
            }
            return result;
        }

        /// <summary>Lấy các thông tin loại hình vận tải</summary>
        /// <returns> Các thông tin loại hình vận tải</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        private async Task<List<BGT_TranportTypes>> GetAllTranportTypes()
        {
            var result = new List<BGT_TranportTypes>();
            try
            {
                var cacheKey = $"GreeterService_GetAllTranportTypes";
                result = await _cacheHelper.GetDataFromCache<BGT_TranportTypes>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var types = _transportTypesRepository.GetAll();
                    result = types.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GreeterService_GetAllTranportTypes_{ex.Message}");
            }
            return result;
        }

        /// <summary>Lấy thông tin xe theo đơn vị vận tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        private async Task<List<Vehicle_Vehicles>> GetAllVehicleByCompany(int input)
        {
            var result = new List<Vehicle_Vehicles>();
            try
            {
                var cacheKey = $"GreeterService_GetAllVehicleByCompany_{input}";
                result = await _cacheHelper.GetDataFromCache<Vehicle_Vehicles>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    var vehicles = _vehiclesRepository.GetAllByColumnId("FK_CompanyID", input, true);
                    result = vehicles.ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GreeterService_GetAllVehicleByCompany_{input}_{ex.Message}");
            }
            return result;
        }
    }
}