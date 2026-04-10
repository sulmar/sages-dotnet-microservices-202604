using Microsoft.AspNetCore.Authentication.JwtBearer;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// dotnet add package Microsoft.Extensions.ServiceDiscovery
builder.Services.AddServiceDiscovery();

// dotnet add package Yarp.ReverseProxy

// dotnet add package Microsoft.Extensions.ServiceDiscovery.Yarp

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver()
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(transformContext =>
        {
            var context = transformContext.HttpContext;

            var token = context.Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(token))
            {
                transformContext.ProxyRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return ValueTask.CompletedTask;
        });
    });

// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


var secretKey = "ThisIsASecretKeyForDemoPurposesOnly";


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "a",
            ValidateAudience = true,
            ValidAudience = "b",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access_token", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(context.Exception.Message);

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("LoggedPolicy", policy => policy.RequireAuthenticatedUser());

    // MVC: [Authorize(Roles= "manager")]
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("manager"));

    options.AddPolicy("CreatorPolicy", policy => policy.RequireClaim("Permission", "create"));
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
