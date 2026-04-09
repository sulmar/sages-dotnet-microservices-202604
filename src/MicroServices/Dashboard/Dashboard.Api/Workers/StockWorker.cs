using Grpc.Core;
using Stock.Api;
using System.Collections;

namespace Dashboard.Api.Workers;

public class StockWorker(Stock.Api.StockService.StockServiceClient client) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var call = client.StreamStockUpdates(new StockUpdateRequest(), cancellationToken: stoppingToken);

            await foreach(var update in call.ResponseStream.ReadAllAsync(stoppingToken))
            {
                Console.WriteLine($"Product {update.ProductId} has {update.AvailableQuantity} items in stock.");
            }            
        }
    }
}


