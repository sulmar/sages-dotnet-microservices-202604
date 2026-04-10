using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Infrastructure.Fakers;
using ProductCatalog.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<Faker<Product>, ProductFaker>();
builder.Services.AddSingleton<ProductCatalogContext>(sp => new ProductCatalogContext { Products = sp.GetRequiredService<Faker<Product>>().Generate(100) });


/*

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
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(context.Exception.Message);
             
                return Task.CompletedTask;
            }

        };
    });

builder.Services.AddAuthorization();

*/

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();


app.MapGet("/", () => "Hello Product Catalog Api!");



app.MapGet("/api/products", async (IProductRepository repository, HttpContext context) =>
{
    //if (!context.User.Identity.IsAuthenticated)
    //{
    //    return Results.Unauthorized();
    //}

    return Results.Ok(repository.GetAll());
});// RequireAuthorization(policy=>policy.RequireRole("manager")); // MVC: [Authorize(Roles="manager"]

app.Run();
