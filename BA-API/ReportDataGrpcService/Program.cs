using Microsoft.AspNetCore.Server.Kestrel.Core;
using ReportDataGrpcService.AutoMapper;
using ReportDataGrpcService.DBContext;
using ReportDataGrpcService.Helpers;
using ReportDataGrpcService.Interfaces.IHelper;
using ReportDataGrpcService.Interfaces.IRepository;
using ReportDataGrpcService.Repository;
using ReportDataGrpcService.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddSingleton<DapperDbContext>();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Mapper
builder.Services.AddAutoMapper(typeof(Mapper));

builder.Services.AddScoped(typeof(ITransportTypesRepository), typeof(TransportTypesRepository));
builder.Services.AddScoped(typeof(IActivitySummariesRepository), typeof(ActivitySummariesRepository));
builder.Services.AddScoped(typeof(IVehicleTransportTypesRepository), typeof(VehicleTransportTypesRepository));
builder.Services.AddScoped(typeof(IVehiclesRepository), typeof(VehiclesRepository));
builder.Services.AddScoped(typeof(ISpeedOversRepository), typeof(SpeedOversRepository));

builder.Services.AddTransient(typeof(ICacheHelper), typeof(CacheHelper));

builder.WebHost.ConfigureKestrel(options =>
{
    // Setup a HTTP/2 endpoint without TLS.
    options.ListenAnyIP(8002, o => o.Protocols = HttpProtocols.Http2);
});

builder.Host.UseSerilog((ctx, lc) => lc
       .WriteTo.Console());

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapGrpcReflectionService();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
