using ThermometerApi;

var builder = WebApplication.CreateBuilder(args);
var app = Startup.Init(builder);
Startup.Run(app);
