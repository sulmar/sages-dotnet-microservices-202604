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
}
