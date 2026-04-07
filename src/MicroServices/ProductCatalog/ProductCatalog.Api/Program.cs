using Bogus;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Infrastructure.Fakers;
using ProductCatalog.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<Faker<Product>, ProductFaker>();
builder.Services.AddSingleton<ProductCatalogContext>(sp => new ProductCatalogContext { Products = sp.GetRequiredService<Faker<Product>>().Generate(100) });

builder.Services.AddCors(options => options.AddPolicy("ProductCatalogPolicy", policy=>
{
    policy.WithOrigins("https://localhost:7108");
    policy.WithMethods("GET");
    policy.AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("ProductCatalogPolicy");

app.MapGet("/", () => "Hello Product Catalog Api!");

app.MapGet("/api/products", (IProductRepository repository) => repository.GetAll());

app.Run();
