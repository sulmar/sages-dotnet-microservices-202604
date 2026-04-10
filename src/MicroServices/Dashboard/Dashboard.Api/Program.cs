using Dashboard.Api.Workers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<StockWorker>();


builder.Services.AddGrpcClient<Stock.Api.StockService.StockServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7118");
});

builder.Services.AddSignalR();

// builder.Logging.AddJsonConsole();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();

var app = builder.Build();

app.MapGet("/", () => "Hello Dashboard!");

app.MapHub<Dashboard.Api.Hubs.StockHub>("/signalr/stock");

app.Run();
