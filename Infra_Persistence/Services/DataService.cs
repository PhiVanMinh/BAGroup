using Application.IService;
using AutoMapper;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using ReportDataGrpcService;
using Services.Common.Core.Models;

namespace Infra_Persistence.Services
{

    /// <summary>Lấy dữ liệu từ các service</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/28/2023   created
    /// </Modified>
    public class DataService : IDataService
    {
        private readonly IConfiguration _configuration;
        private readonly IRedisCacheHelper _cacheHelper;
        private readonly IHttpRequestHelper _httpHelper;
        private readonly IMapper _mapper;
        private readonly ILogger<DataService> _logger;

        private readonly GrpcChannel _channel;
        private readonly Greeter.GreeterClient _client;
        private readonly string _connect;

        public DataService(
            ILogger<DataService> logger,
            IConfiguration configuration,
            IRedisCacheHelper cacheHelper,
            IHttpRequestHelper httpHelper,
            IMapper mapper
            )
        {
            _logger = logger;
            _configuration = configuration;
            _cacheHelper = cacheHelper;
            _httpHelper = httpHelper;
            _mapper = mapper;

            _channel = GrpcChannel.ForAddress(_configuration["GrpcSettings:ReportDataServiceUrl"]);
            _client = new Greeter.GreeterClient(_channel);

            _connect = _configuration["MicroService:Connection"];

        }

        /// <summary>Loại hình vận tải</summary>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/28/2023   created
        /// </Modified>
        public async Task<IEnumerable<BGT_TranportTypes>> GetTransportTypes()
        {
            var result = new List<BGT_TranportTypes>();
            try
            {
                var cacheKey = $"DataService_GetTransportTypes";
                result = await _cacheHelper.GetDataFromCache<BGT_TranportTypes>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    if (_connect == "http")
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseRest = await _httpHelper.GetDataFromOtherService<BGT_TranportTypes>($"{_configuration["UrlBase"]}/Vehicles/transport-type");
                        result = reponseRest.ToList();
                        watch.Stop();
                        Console.WriteLine($"Execution BGT_TranportTypes by Rest Time: {watch.ElapsedMilliseconds} ms");
                        //var reponseRest = await _httpHelper.GetDataFromOtherService<BGT_TranportTypes>($"{_configuration["UrlBase"]}/Vehicles/transport-type");
                        result = reponseRest.ToList();
                    }
                    else
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseGrpc = _client.GetTransportTypes(new Empty { });
                        result = _mapper.Map<List<BGT_TranportTypes>>(reponseGrpc.Items);
                        watch.Stop();
                        Console.WriteLine($"Execution BGT_TranportTypes by grpc Time: {watch.ElapsedMilliseconds} ms");
                    }
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return result;
        }

        /// <summary>Lấy thông tin xe gắn với loại hình vận tải</summary>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/28/2023   created
        /// </Modified>
        public async Task<IEnumerable<BGT_VehicleTransportTypes>> GetVehicleTransportType()
        {
            var result = new List<BGT_VehicleTransportTypes>();
            try
            {
                var cacheKey = $"DataService_GetVehicleTransportType";
                result = await _cacheHelper.GetDataFromCache<BGT_VehicleTransportTypes>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    if (_connect == "http")
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseRest = await _httpHelper.GetDataFromOtherService<BGT_VehicleTransportTypes>($"{_configuration["UrlBase"]}/Vehicles/vehicle-type");
                        watch.Stop();
                        Console.WriteLine($"Execution BGT_VehicleTransportTypes Time: {watch.ElapsedMilliseconds} ms");
                        result = reponseRest.ToList();
                    }
                    else
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseGrpc = _client.GetVehicleTransportType(new Empty { });
                        result = _mapper.Map<List<BGT_VehicleTransportTypes>>(reponseGrpc.Items);
                        watch.Stop();
                        Console.WriteLine($"Execution BGT_VehicleTransportTypes by grpc Time: {watch.ElapsedMilliseconds} ms");
                    }
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return result;
        }

        /// <summary>Lấy thông tin xe</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>
        ///   <para>Thông tin các xe</para>
        /// </returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/28/2023   created
        /// </Modified>
        public async Task<IEnumerable<Vehicle_Vehicles>> GetVehicleInfo(int input)
        {
            var result = new List<Vehicle_Vehicles>();
            try
            {
                var cacheKey = $"DataService_GetVehicleInfo_{input}";
                result = await _cacheHelper.GetDataFromCache<Vehicle_Vehicles>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    if (_connect == "http")
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseRest = await _httpHelper.GetDataFromOtherService<Vehicle_Vehicles>
                                                ($"{_configuration["UrlBase"]}/Vehicles/vehicle?input={input}");
                        watch.Stop();
                        Console.WriteLine($"Execution Vehicle_Vehicles Time: {watch.ElapsedMilliseconds} ms");
                        result = reponseRest.ToList();
                    }
                    else
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseGrpc = _client.GetVehicleInfo(new GetById { Id = input });
                        result = _mapper.Map<List<Vehicle_Vehicles>>(reponseGrpc.Items);
                        watch.Stop();
                        Console.WriteLine($"Execution Vehicle_Vehicles by grpc Time: {watch.ElapsedMilliseconds} ms");
                    }
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return result;
        }

        /// <summary>Thông tin tổng hợp</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin tổng hợp</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/28/2023   created
        /// </Modified>
        public async Task<IEnumerable<Report_ActivitySummaries>> GetActivitySummaries(int input)
        {
            var result = new List<Report_ActivitySummaries>();
            try
            {
                var cacheKey = $"DataService_GetActivitySummaries_{input}";
                result = await _cacheHelper.GetDataFromCache<Report_ActivitySummaries>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    if (_connect == "http")
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseRest = await _httpHelper.GetDataFromOtherService<Report_ActivitySummaries>
                                                ($"{_configuration["UrlBase"]}/Vehicles/activiti-summary?input={input}");
                        watch.Stop();
                        Console.WriteLine($"Execution Report_ActivitySummaries Time: {watch.ElapsedMilliseconds} ms");
                        result = reponseRest.ToList();
                    }
                    else
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseGrpc = _client.GetActivitySummaries(new GetById { Id = input });
                        result = _mapper.Map<List<Report_ActivitySummaries>>(reponseGrpc.Items);
                        watch.Stop();
                        Console.WriteLine($"Execution Report_ActivitySummaries by grpc Time: {watch.ElapsedMilliseconds} ms");
                    }
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return result;
        }

        /// <summary>Thông tin vi phạm tốc độ</summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Thông tin vi phạm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/28/2023   created
        /// </Modified>
        public async Task<IEnumerable<BGT_SpeedOvers>> GetSpeedOvers(DateTime fromDate, DateTime toDate)
        {
            var result = new List<BGT_SpeedOvers>();
            try
            {
                var cacheKey = $"DataService_GetSpeedOvers_{fromDate}_{toDate}";
                result = await _cacheHelper.GetDataFromCache<BGT_SpeedOvers>(cacheKey, 0, 0);
                if (result.Count() == 0)
                {
                    if (_connect == "http")
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseRest = await _httpHelper.GetDataFromOtherService<BGT_SpeedOvers>
                                                ($"{_configuration["UrlBase"]}/Vehicles/speedOver?fromDate={fromDate}&toDate={toDate}");
                        watch.Stop();
                        Console.WriteLine($"Execution BGT_SpeedOvers Time: {watch.ElapsedMilliseconds} ms");
                        result = reponseRest.ToList();
                    }
                    else
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        var reponseGrpc = _client.GetSpeedOver(new GetSpeedOverRequest
                        {
                            FromDate = fromDate.ToString(),
                            ToDate = toDate.ToString(),
                        });
                        result = _mapper.Map<List<BGT_SpeedOvers>>(reponseGrpc.Items);
                        watch.Stop();
                        Console.WriteLine($"Execution BGT_SpeedOvers by grpc Time: {watch.ElapsedMilliseconds} ms");
                    }
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return result;
        }

    }
}
