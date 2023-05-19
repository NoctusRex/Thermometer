using Configurations;
using DhtApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Repositories;
using System.Text;
using Utils;

Logger logger;
ApplicationConfiguration applicationConfiguration;
var builder = WebApplication.CreateBuilder(args);

InitLogging();
InitConfig();
InitSwaggerServices();
InitAuthentication();
RegisterRepositories();

var app = builder.Build();

InitCors();
InitSwaggerApp();

RegisterRoutes();

Run();

void InitLogging()
{
    string logConfig = Path.Combine(PathManager.ConfigurationDirectory, "log.config");
    if (!File.Exists(logConfig))
        File.Copy(Path.Combine(PathManager.StartupDirectory, Path.Combine("Properties", "nlog.config")), logConfig);
    logger = NLogBuilder.ConfigureNLog(logConfig).GetCurrentClassLogger();
#pragma warning disable ASP0011 // Suggest using builder.Logging over Host.ConfigureLogging or WebHost.ConfigureLogging
    builder.Host.ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
        logging.AddNLog(logConfig);
    });
#pragma warning restore ASP0011 // Suggest using builder.Logging over Host.ConfigureLogging or WebHost.ConfigureLogging

    logger.Debug("InitLogging");
}

void InitConfig()
{
    logger.Debug("InitConfig");

    applicationConfiguration = ConfigurationLoader.Get("Application.config.json", new ApplicationConfiguration());
    builder.Services.AddSingleton(applicationConfiguration);
}

void InitCors()
{
    if (!applicationConfiguration.UseCors)
    {
        logger.Debug("Cors disabled");
        return;
    }

    logger.Debug("InitCors");

    builder.Services.AddCors(o =>
        o.AddDefaultPolicy(b =>
            b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    app.UseCors();
}

void InitSwaggerServices()
{
    logger.Debug("InitSwaggerServices");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

void InitSwaggerApp()
{
    if (!app.Environment.IsDevelopment())
    {
        logger.Debug("Swagger disabled");
        return;
    }

    logger.Debug("InitSwaggerApp");

    app.UseSwagger();
    app.UseSwaggerUI();
}

void InitAuthentication()
{
    logger.Debug("InitAuthentication");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = applicationConfiguration.JwtIssuer,
            ValidAudience = applicationConfiguration.JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(applicationConfiguration.JwtKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };
    });
    builder.Services.AddAuthorization();
    builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
}

void Run()
{
    string url = $"{(applicationConfiguration.UseHttps ? "https" : "http")}://*:{applicationConfiguration.Port}";
    logger.Info($"Running on: {url}");

    app.Run(url);
}

void RegisterRepositories()
{
    logger.Debug("Register repositories");

    builder.Services.AddTransient<DatabaseRepository>();
    builder.Services.AddTransient<DataRepository>();
}

void RegisterRoutes()
{
    logger.Debug("Register routes");

    app.MapGet("/set", (decimal temperature, decimal humidity) =>
    {
        app.Services.GetService<DataRepository>()!.Set(temperature, humidity);
    });

    app.MapPut("/get", (MeasurementQuery query) =>
    {
        return app.Services.GetService<DataRepository>()!.Get(query);
    });
}