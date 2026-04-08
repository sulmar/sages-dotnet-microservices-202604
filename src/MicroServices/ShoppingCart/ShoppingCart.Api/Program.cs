using ShoppingCart.Domain;
using ShoppingCart.Domain.Repositories;
using ShoppingCart.Infrastructure.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddScoped<ICartItemRepository, RedisCartItemRepository>();

builder.Services.AddCors(options => options.AddPolicy("CartPolicy", policy =>
{
    policy.WithOrigins("https://localhost:7108");
    policy.WithMethods("GET");
    policy.AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("CartPolicy");

app.MapGet("/", () => "Hello Shopping Cart Api!");

app.MapPost("/api/cart/items", (Product product, ICartItemRepository repository, HttpContext context) =>
{    
    var sessionId = context.User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value ?? "user:" + "1";

    repository.AddAsync(sessionId, product);
});

app.Run();
