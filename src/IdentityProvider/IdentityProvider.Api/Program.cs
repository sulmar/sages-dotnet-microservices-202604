using IdentityProvider.Api.Domain;
using IdentityProvider.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthService, FakeAuthService>();
builder.Services.AddScoped<ITokenService, FakeTokenService>();
builder.Services.AddScoped<IUserRepository, FakeUserRepository>();

var app = builder.Build();

app.MapGet("/", () => "Hello Identity Provider!");

app.MapPost("/api/login", async (LoginRequest request, IAuthService authService, ITokenService tokenService, HttpContext context) =>
{
     var result = await authService.ValidateAsync(request.Username, request.Password);
 
    if (result.IsValid)
    {
        var accessToken = tokenService.GenerateToken(result.Identity);

        context.Response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true, // brak dostepu z JavaScript
            Secure = true, // tylko HTTPS
            SameSite = SameSiteMode.Strict, // tylko ta s
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Results.Ok(new { AccessToken = accessToken });
    }
    
    return Results.Unauthorized();

});

app.Run();


public record LoginRequest(string Username, string Password);