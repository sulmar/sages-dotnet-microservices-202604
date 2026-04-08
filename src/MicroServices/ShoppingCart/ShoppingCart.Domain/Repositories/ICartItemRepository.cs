namespace ShoppingCart.Domain.Repositories;

public interface ICartItemRepository
{
    Task AddAsync(string SessionId, Product product);
    Task<IEnumerable<CartItem>> GetItemsAsync(string SessionId);
}
