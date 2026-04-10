using Grpc.Core;

namespace Stock.Api.Services;

public class StockGrpcService : StockService.StockServiceBase
{
    private readonly Dictionary<int, int> _stock = new()
    {
        { 1, 10 },
        { 2, 5  },
        { 3, 10 },
        { 4, 10 },
        { 5, 10 }
    };

    public override Task<CheckAvailabilityResponse> CheckAvailability(CheckAvailabilityRequest request, ServerCallContext context)
    {
        // ...        
        var available = _stock.TryGetValue(request.ProductId, out var quantity) && quantity >= request.Quantity;

        // > HGET stock:warszawa product:1

        var response = new CheckAvailabilityResponse
        {
            IsAvailable = available
        };

        return Task.FromResult(response);

    }

    public override Task<ReserveStockResponse> ReserveStock(ReserveStockRequest request, ServerCallContext context)
    {
        return base.ReserveStock(request, context);
    }


    public override async Task StreamStockUpdates(StockUpdateRequest request, IServerStreamWriter<StockUpdateResponse> responseStream, 
        ServerCallContext context)
    {
        while(!context.CancellationToken.IsCancellationRequested)
        {
            var update = new StockUpdateResponse
            {
                ProductId = Random.Shared.Next(1, 3),
                AvailableQuantity = Random.Shared.Next(0, 10)
            };

            await responseStream.WriteAsync(update);

            Console.WriteLine($"Sent Product {update.ProductId} has {update.AvailableQuantity} items in stock.");

            // co 1 sekundę wysyłaj aktualizację
            await Task.Delay(Random.Shared.Next(1000, 3000), context.CancellationToken);
        }

        
    }
}
