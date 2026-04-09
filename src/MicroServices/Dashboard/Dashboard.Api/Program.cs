using Dashboard.Api.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<StockWorker>();


builder.Services.AddGrpcClient<Stock.Api.StockService.StockServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7118");
});

var app = builder.Build();

app.MapGet("/", () => "Hello Dashboard!");

app.Run();
