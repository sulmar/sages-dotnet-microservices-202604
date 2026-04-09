using Grpc.Core;
using Stock.Api;
using System.Collections;

namespace Dashboard.Api.Workers;

public class StockWorker(Stock.Api.StockService.StockServiceClient client, ILogger<StockWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var call = client.StreamStockUpdates(new StockUpdateRequest(), cancellationToken: stoppingToken);

            await foreach(var update in call.ResponseStream.ReadAllAsync(stoppingToken))
            {
                // zla praktyka
                // logger.LogInformation($"Product {update.ProductId} has {update.AvailableQuantity} items in stock.");

                // dobra praktyka
                logger.LogInformation("Product {ProductId} has {AvailableQuantity} items in stock.", update.ProductId, update.AvailableQuantity);
            }            
        }
    }
}


