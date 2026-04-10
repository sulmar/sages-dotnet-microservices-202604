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


// dotned add package AspNetCore.HealthChecks.Redis
builder.Services.AddHealthChecks()    
    .AddCheck("random", ()=>
    {
        if (DateTime.Now.Minute % 2 == 0)
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Random failure");
        else
             return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();

    })
    .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>(), name: "redis", tags: new[] { "ready" });

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


app.MapHealthChecks("/hc", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.Run();
