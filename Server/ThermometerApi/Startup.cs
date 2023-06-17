using Configurations;
using DhtApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLog;
using Microsoft.AspNetCore.Routing;
using Utils;
using NLog.Web;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ThermometerApi
{
    public class Startup
    {
        private static ApplicationConfiguration ApplicationConfiguration { get; set; } = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static Logger Logger { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static WebApplication Init(WebApplicationBuilder builder)
        {
            InitLogging(builder);
            ConfigureServices(builder.Services);

            var app = builder.Build();
            Configure(app);

            return app;
        }

        public static void Run(WebApplication app)
        {
            string url = $"{(ApplicationConfiguration.UseHttps ? "https" : "http")}://*:{ApplicationConfiguration.Port}";
            Logger.Info($"Running on: {url}");

            app.Run(url);
        }

        public static Task RunAsync(WebApplication app)
        {
            string url = $"{(ApplicationConfiguration.UseHttps ? "https" : "http")}://*:{ApplicationConfiguration.Port}";
            Logger.Info($"Running on: {url}");

            return app.RunAsync(url);
        }


        static void InitLogging(WebApplicationBuilder builder)
        {
            string logConfig = Path.Combine(PathManager.ConfigurationDirectory, "log.config");
            if (!File.Exists(logConfig))
                File.Copy(Path.Combine(PathManager.StartupDirectory, Path.Combine("Properties", "nlog.config")), logConfig);
            Logger = NLogBuilder.ConfigureNLog(logConfig).GetCurrentClassLogger();
#pragma warning disable ASP0011 // Suggest using builder.Logging over Host.ConfigureLogging or WebHost.ConfigureLogging
            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                logging.AddNLog(logConfig);
            });
#pragma warning restore ASP0011 // Suggest using builder.Logging over Host.ConfigureLogging or WebHost.ConfigureLogging

            Logger.Warn("InitLogging");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            InitConfig(services);
            InitCorsService(services);
            InitSwaggerServices(services);
            InitAuthentication(services);
            RegisterRepositories(services);
        }

        static void Configure(WebApplication app)
        {
            InitCors(app);
            InitSwaggerApp(app);
            RegisterRoutes(app);
        }

        static void InitConfig(IServiceCollection services)
        {
            Logger.Warn("InitConfig");

            ApplicationConfiguration = ConfigurationLoader.Get("Application.config.json", new ApplicationConfiguration());
            services.AddSingleton(ApplicationConfiguration);
        }

        static void InitCors(IApplicationBuilder app)
        {
            if (!ApplicationConfiguration.UseCors) return;

            Logger.Warn("InitCors");
            app.UseCors();
        }

        static void InitSwaggerApp(IApplicationBuilder app)
        {
            if (!Debugger.IsAttached)
            {
                Logger.Warn("Swagger disabled");
                return;
            }

            Logger.Warn("InitSwaggerApp");

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        static void InitCorsService(IServiceCollection services)
        {
            if (!ApplicationConfiguration.UseCors)
            {
                Logger.Warn("Cors disabled");
                return;
            }

            Logger.Warn("InitCors Services");

            services.AddCors(o =>
                o.AddDefaultPolicy(b =>
                    b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        }

        static void InitSwaggerServices(IServiceCollection services)
        {
            Logger.Warn("InitSwaggerServices");

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        static void InitAuthentication(IServiceCollection services)
        {
            Logger.Warn("InitAuthentication");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = ApplicationConfiguration.JwtIssuer,
                    ValidAudience = ApplicationConfiguration.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(ApplicationConfiguration.JwtKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });
            services.AddAuthorization();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        static void RegisterRepositories(IServiceCollection services)
        {
            Logger.Warn("Register repositories");

            services.AddTransient<DatabaseRepository>();
            services.AddTransient<DataRepository>();
        }

        static void RegisterRoutes(WebApplication app)
        {
            Logger.Warn("Register routes");

            app.MapGet("/set", (decimal temperature, decimal humidity, string deviceName) =>
            {
                app.Services.GetService<DataRepository>()!.Set(temperature, humidity, deviceName);
            });

            app.MapPut("/get", (MeasurementQuery query) =>
            {
                return app.Services.GetService<DataRepository>()!.Get(query);
            });

            app.MapGet("/get-device-names", () =>
            {
                return app.Services.GetService<DataRepository>()!.GetDeviceNames();
            });
        }
    }
}
