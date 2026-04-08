using NanoidDotNet;
using Ordering.Domain;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello Ordering Api!");

app.MapPost("/api/orders", (Order order) =>
{
    // Here you would typically save the order to a database
    // For this example, we'll just return the order back to the client
    // order.Id = Guid.NewGuid().ToString();

    // dotnet add package NanoId
    order.Id = Nanoid.Generate(size: 5);

    // TODO: zapisz do bazy danych

    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();
