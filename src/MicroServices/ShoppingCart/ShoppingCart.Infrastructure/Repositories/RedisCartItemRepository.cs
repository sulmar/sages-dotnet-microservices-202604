using ShoppingCart.Domain;
using ShoppingCart.Domain.Repositories;
using StackExchange.Redis;

namespace ShoppingCart.Infrastructure.Repositories;

// dotnet add package StackExchange.Redis
public class RedisCartItemRepository(IConnectionMultiplexer connection): ICartItemRepository
{
    public async Task AddAsync(string SessionId, Product product)
    { 
        var key = $"cart:{SessionId}";
        var field = $"product:{product.Id}";

        var db = connection.GetDatabase();

        // HINCRBY cart:{session:id} {product:id} {quantity}       
        await db.HashIncrementAsync(key, field, 1);

        // EXPIRE cart:{session:id} 120
        await db.KeyExpireAsync(key, TimeSpan.FromMinutes(2));
    }

    public async Task<IEnumerable<CartItem>> GetItemsAsync(string SessionId)
    {
        var key = $"cart:{SessionId}";

        var db = connection.GetDatabase();

        // HGETALL cart:user:1 
        var entries = await db.HashGetAllAsync(key);

        // Mapowanie wyników do listy CartItem
        var cartItems = entries.Select(entry => new CartItem
        {
            ProductId = int.Parse(entry.Name.ToString().Split(':')[1]),
            Quantity = (int)entry.Value
        });

        return cartItems;
    }
}
