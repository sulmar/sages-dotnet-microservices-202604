using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public Category Category { get; set; }
}


public class Category
{
    public string Name { get; set; }
}