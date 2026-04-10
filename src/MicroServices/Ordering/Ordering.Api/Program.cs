using NanoidDotNet;
using Ordering.Api;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddScoped<OrderProcessor>();

builder.Services.AddGrpcClient<Stock.Api.StockService.StockServiceClient>(options => options.Address = new Uri("https://localhost:7118"));

var app = builder.Build();

app.MapGet("/", () => "Hello Ordering Api!");

app.MapPost("/api/orders", async (Ordering.Domain.Order order, OrderProcessor orderProcessor) =>
{
    try
    {
        await orderProcessor.ProcessAsync(order);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }

    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();

