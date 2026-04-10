using NanoidDotNet;
using Ordering.Api;
using Ordering.Api.Workers;
using StackExchange.Redis;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddSingleton<OrderProcessor>();
builder.Services.AddHostedService<OrderWorker>();
builder.Services.AddSingleton(Channel.CreateUnbounded<Ordering.Domain.Order>());
//builder.Services.AddSingleton(Channel.CreateBounded<Ordering.Domain.Order>(10));

builder.Services.AddGrpcClient<Stock.Api.StockService.StockServiceClient>(options => options.Address = new Uri("https://localhost:7118"));

var app = builder.Build();

app.MapGet("/", () => "Hello Ordering Api!");

app.MapPost("/api/orders", async (Ordering.Domain.Order order, Channel<Ordering.Domain.Order> channel) =>
{
    try
    {
        await channel.Writer.WriteAsync(order);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }

    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();

