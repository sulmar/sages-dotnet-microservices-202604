using NanoidDotNet;
using Ordering.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<Stock.Api.StockService.StockServiceClient>(options => options.Address = new Uri("https://localhost:7118"));

var app = builder.Build();

app.MapGet("/", () => "Hello Ordering Api!");

app.MapPost("/api/orders", async (Order order, Stock.Api.StockService.StockServiceClient stockClient) =>
{    
    foreach (var item in order.Items)
    {
        var request = new Stock.Api.CheckAvailabilityRequest
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        };

        var response =  stockClient.CheckAvailability(request);
        
        if (!response.IsAvailable)
        {
            return Results.BadRequest($"Product {item.ProductId} is not available in the requested quantity.");
        }
    }

        



    // Here you would typically save the order to a database
    // For this example, we'll just return the order back to the client
    // order.Id = Guid.NewGuid().ToString();

    // dotnet add package NanoId
    order.Id = Nanoid.Generate(size: 5);

    // TODO: zapisz do bazy danych

    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();
