namespace ShoppingCart.Domain.Repositories;

public interface ICartItemRepository
{
    Task AddAsync(string SessionId, Product product);
    Task RemoveAsync(string SessionId, int productId);
    Task<IEnumerable<CartItem>> GetItemsAsync(string SessionId);
}
