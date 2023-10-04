﻿using Application.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infra_Persistence.Helper
{
    /// <summary>Lấy DL qua HTTP Client</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/19/2023   created
    /// </Modified>
    public class HttpRequestHelper : IHttpRequestHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HttpRequestHelper> _logger;

        public HttpRequestHelper(IConfiguration configuration, ILogger<HttpRequestHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>Lấy dữ liệu từ bên service khác</summary>
        /// <typeparam name="T">Kiểu dữ liệu cần trả</typeparam>
        /// <param name="link">Link api</param>
        /// <returns>Danh sách dữ liệu</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public async Task<IEnumerable<T>> GetDataFromOtherService<T>(string link)
        {
            var result = new List<T>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                        var option = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    var response = await httpClient.PostAsync(link, null);
                    watch.Stop();
                    if(link.Contains("/Vehicles/transport-type")) Console.WriteLine($"Execution transportType Time: {watch.ElapsedMilliseconds} ms");
                    if (link.Contains("/Vehicles/speedOver")) Console.WriteLine($"Execution speedOver Time: {watch.ElapsedMilliseconds} ms");
                    string apiResponse = await response.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<List<T>>(apiResponse, option) ?? new List<T>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetData_{link}_{ex.Message}");
            }

            return result;
        }
    }
}
