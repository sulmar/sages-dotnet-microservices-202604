var builder = WebApplication.CreateBuilder(args);

// dotnet add package Yarp.ReverseProxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();


/*

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

*/


app.MapGet("/test", () => "Hello Test Api!");

app.Run();
