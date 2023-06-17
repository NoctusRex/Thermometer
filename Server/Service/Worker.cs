using Microsoft.AspNetCore.Builder;

namespace Service
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var builder = WebApplication.CreateBuilder();
                var app = ThermometerApi.Startup.Init(builder);

                await ThermometerApi.Startup.RunAsync(app);
            }
            catch
            {
                // Handle any exceptions here
                // You can log the error or perform other actions
            }
        }
    }
}