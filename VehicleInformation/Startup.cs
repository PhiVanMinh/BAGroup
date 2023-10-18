using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ReportSpeedOver.API.Common.Helpers;
using ReportSpeedOver.API.Common.Interfaces.IHelper;
using Serilog;
using VehicleInformation.DbContext;
using VehicleInformation.Interfaces.IRepository;
using VehicleInformation.Interfaces.IService;
using VehicleInformation.Repository;
using VehicleInformation.Services;

namespace VehicleInformation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();

            services.AddSingleton<DapperContext>();

            services.AddScoped(typeof(ITransportTypesRepository), typeof(TransportTypesRepository));
            services.AddScoped(typeof(IActivitySummariesRepository), typeof(ActivitySummariesRepository));
            services.AddScoped(typeof(IVehicleTransportTypesRepository), typeof(VehicleTransportTypesRepository));
            services.AddScoped(typeof(IVehiclesRepository), typeof(VehiclesRepository));
            services.AddScoped(typeof(ISpeedOversRepository), typeof(SpeedOversRepository));

            services.AddTransient(typeof(ITransportTypesService), typeof(TransportTypesService));
            services.AddTransient(typeof(IActivitySummariesService), typeof(ActivitySummariesService));
            services.AddTransient(typeof(IVehicleTransportTypesService), typeof(VehicleTransportTypesService));
            services.AddTransient(typeof(IVehiclesService), typeof(VehiclesService));
            services.AddTransient(typeof(ISpeedOversService), typeof(SpeedOversService));

            services.AddTransient(typeof(IRedisCacheHelper), typeof(RedisCacheHelper));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Implement Swagger UI",
                    Description = "A simple example to Implement Swagger UI",
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Showing API V1");
                });

            }
        }
    }
}
