var builder = WebApplication.CreateBuilder(args);

// dotnet add package Microsoft.Extensions.ServiceDiscovery
builder.Services.AddServiceDiscovery();

// dotnet add package Yarp.ReverseProxy

// dotnet add package Microsoft.Extensions.ServiceDiscovery.Yarp

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddAuthentication("Bearer");

builder.Services.AddAuthorization(options => {
    options.AddPolicy("LoggedPolicy", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

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
