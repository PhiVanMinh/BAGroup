using Application.Interfaces;
using Application.IService;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.CustomMapper;
using Infrastructures.Service;
using Serilog;
using Infrastructures.Authorization;

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
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Auth0:Issuer"],
            ValidAudience = builder.Configuration["Auth0:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth0:SecretKey"])),
            ClockSkew = TimeSpan.Zero
        };
        builder.Services.AddCors();
    });

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy(Policies.UserView, Policies.ViewPolicy());
    config.AddPolicy(Policies.UserCreate, Policies.CreatePolicy());
    config.AddPolicy(Policies.UserUpdate, Policies.UpdatePolicy());
    config.AddPolicy(Policies.UserDelete, Policies.DeletePolicy());
    config.AddPolicy(Policies.CreateOrUpdateUser, Policies.CreateOrUpdatePolicy());
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

