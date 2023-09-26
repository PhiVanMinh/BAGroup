using ReportDataGrpcService.DBContext;
using ReportDataGrpcService.Interfaces.IRepository;
using ReportDataGrpcService.Repository;
using ReportDataGrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DapperDbContext>();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddScoped(typeof(ITransportTypesRepository), typeof(TransportTypesRepository));
builder.Services.AddScoped(typeof(IActivitySummariesRepository), typeof(ActivitySummariesRepository));
builder.Services.AddScoped(typeof(IVehicleTransportTypesRepository), typeof(VehicleTransportTypesRepository));
builder.Services.AddScoped(typeof(IVehiclesRepository), typeof(VehiclesRepository));
builder.Services.AddScoped(typeof(ISpeedOversRepository), typeof(SpeedOversRepository));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
