using Grpc.Core;

namespace Stock.Api.Services;

public class StockGrpcService : StockService.StockServiceBase
{
    private readonly Dictionary<int, int> _stock = new()
    {
        { 1, 10 },
        { 2, 5  },
        { 3, 1 }
    };

    public override Task<CheckAvailabilityResponse> CheckAvailability(CheckAvailabilityRequest request, ServerCallContext context)
    {
        // ...        
        var available = _stock.TryGetValue(request.ProductId, out var quantity) && quantity >= request.Quantity;

        var response = new CheckAvailabilityResponse
        {
            IsAvailable = available
        };

        return Task.FromResult(response);

    }
}
