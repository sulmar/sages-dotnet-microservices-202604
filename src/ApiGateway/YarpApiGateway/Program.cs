var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Logger Middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

// Secret Key Middleware
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Secret-Key", out var secretValue) && secretValue == "your-secret-key")
    {
        await next();
    }
    else
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
    }
});


app.MapGet("/", () => "Hello Gateway Api!");
app.MapGet("/test", () => "Hello Test Api!");

app.Run();
