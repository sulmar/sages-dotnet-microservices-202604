using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Repositories;

// ISP - Interface Segregation Principle

public interface IProductRepository
{
    List<Product> GetAll();
    List<Product> GetByCategory(string category);
    Product GetById(int id);    
}
