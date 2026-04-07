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
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Price, f => f.Random.Decimal(1, 100));
        RuleFor(p => p.DiscountedPrice, (f, p) => f.Random.Decimal(0, p.Price));
    }
}
