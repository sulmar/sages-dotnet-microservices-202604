using NanoidDotNet;
using StackExchange.Redis;

namespace Ordering.Api;

public class OrderProcessor(Stock.Api.StockService.StockServiceClient stockClient, IConnectionMultiplexer redis)
{
    public async Task ProcessAsync(Domain.Order order)
    {
        foreach (var item in order.Items)
        {
            var request = new Stock.Api.CheckAvailabilityRequest
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };

            var response = stockClient.CheckAvailability(request);

            if (!response.IsAvailable)
            {
                throw new InvalidOperationException($"Product {item.ProductId} is not available in the requested {item.Quantity} quantity.");
            }
        }


        // Here you would typically save the order to a database
        // For this example, we'll just return the order back to the client
        // order.Id = Guid.NewGuid().ToString();

        // dotnet add package NanoId
        order.Id = Nanoid.Generate(size: 5);

        // TODO: asynchroniczne sprawdzenie platnosci
        var db = redis.GetDatabase();

        // XADD orders-stream * type order-placed orderId {order.Id} amount 500 status pending
        await db.StreamAddAsync("orders-stream",
        [
            new NameValueEntry("type", "order-placed"),
        new NameValueEntry("orderId", order.Id),
        new NameValueEntry("amount", order.TotalAmount.ToString()),
        new NameValueEntry("status", "pending")
        ]);
    }
}
