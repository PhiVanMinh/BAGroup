using Application.Interfaces;
using Application.IService;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Infrastructure.CustomMapper;
using Infrastructures.Service;
using Serilog;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//Logger
var logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mapper
builder.Services.AddAutoMapper(typeof(CustomMapper));

// Service
builder.Services.AddTransient(typeof(IUserService), typeof(UserService));
builder.Services.AddTransient(typeof(IAuthService), typeof(AuthService));

builder.Services.AddCors();

// Dependency Injection
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
//    options.TokenValidationParameters =
//      new TokenValidationParameters
//      {
//          ValidAudience = builder.Configuration["Auth0:Audience"],
//          ValidIssuer = $"{builder.Configuration["Auth0:Domain"]}",
//          ValidateLifetime = true,
//      };
//});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("WriteAccess", policy => policy.RequireClaim(ClaimTypes.Role, "user.create", "user.update"));
//    options.AddPolicy("DeleteAccess", policy => policy.RequireClaim(ClaimTypes.Role, "user.delete"));
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

