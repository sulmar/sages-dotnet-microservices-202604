using Bogus;
using ProductCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Infrastructure.Fakers;

// dotnet add package Bogus

public class ProductFaker : Faker<Product>
{
    public ProductFaker()
    {
        RuleFor(p => p.Id, f => f.IndexFaker + 1);
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Price, f => Math.Round( f.Random.Decimal(1, 100), 2));
        RuleFor(p => p.DiscountedPrice, (f, p) => Math.Round( f.Random.Decimal(0, p.Price), 2));
    }
}
