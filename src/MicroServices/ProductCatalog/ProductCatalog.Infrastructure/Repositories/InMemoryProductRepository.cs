using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Infrastructure.Repositories;

public class ProductCatalogContext
{
    public List<Product> Products { get; set; } = new List<Product>();
}

// Primary Constructor
public class InMemoryProductRepository(ProductCatalogContext _context) : IProductRepository
{
    public List<Product> GetAll() => _context.Products;
    public List<Product> GetByCategory(string category) => _context.Products.Where(p => p.Category.Name == category).ToList();
    public Product GetById(int id) => _context.Products.ElementAtOrDefault(id);
}
