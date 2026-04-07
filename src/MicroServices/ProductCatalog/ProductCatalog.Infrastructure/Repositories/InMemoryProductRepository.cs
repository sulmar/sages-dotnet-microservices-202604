using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Infrastructure.Repositories;

public class ProductCatalogContext
{
    public List<Product> Products { get; set; } = new List<Product>();
}


public class InMemoryProductRepository : IProductRepository
{
    private ProductCatalogContext _context;

    public InMemoryProductRepository(ProductCatalogContext context)
    {
        _context = context;
    }

    public List<Product> GetAll()
    {
        return _context.Products;
    }

    public List<Product> GetByCategory(string category)
    {
        return _context.Products.Where(p => p.Category.Name == category).ToList();
    }

    public Product GetById(int id)
    {
        return _context.Products.ElementAtOrDefault(id);
    }
}
