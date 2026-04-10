using ShoppingCart.Domain;
using ShoppingCart.Domain.Repositories;
using ShoppingCart.Infrastructure.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddScoped<ICartItemRepository, RedisCartItemRepository>();
builder.Services.AddScoped<ICartService, CartService>();

// dotnet add package Microsoft.Extensions.ServiceDiscovery
builder.Services.AddServiceDiscovery();

builder.Services.AddHttpClient("OrderingApi", client => client.BaseAddress = new Uri("https://ordering"))
    .AddServiceDiscovery();

var app = builder.Build();


app.MapGet("/", () => "Hello Shopping Cart Api!");

app.MapPost("/api/cart/items", async (Product product, ICartItemRepository repository, HttpContext context) =>
{    
    var sessionId = context.User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value ?? "user:" + "1";

    await repository.AddAsync(sessionId, product);
});

app.MapGet("/api/cart/items", async (ICartItemRepository repository, HttpContext context) =>
{
    var sessionId = context.User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value ?? "user:" + "1";
    var items = await repository.GetItemsAsync(sessionId);
    return Results.Ok(items);
});

app.MapDelete("/api/cart/items/{productId:int}", async (int productId, ICartItemRepository repository, HttpContext context) =>
{
    var sessionId = context.User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value ?? "user:" + "1";
    await repository.RemoveAsync(sessionId, productId);
    return Results.NoContent();
});

app.MapPost("api/cart/checkout", async (ICartService cartService, HttpContext context) =>
{
    var sessionId = context.User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value ?? "user:" + "1";

    await cartService.Checkout(sessionId);
});

app.Run();
