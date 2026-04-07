using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Repositories;

public interface ICategoryRepository
{
    List<Category> GetAll();
}
