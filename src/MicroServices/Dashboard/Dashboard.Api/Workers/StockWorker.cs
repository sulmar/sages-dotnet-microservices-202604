using Dashboard.Api.Hubs;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Stock.Api;

namespace Dashboard.Api.Workers;

public class StockWorker(Stock.Api.StockService.StockServiceClient client,
    ILogger<StockWorker> logger,
    IHubContext<StockHub> hub) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var call = client.StreamStockUpdates(new StockUpdateRequest(), cancellationToken: stoppingToken);

                await foreach (var update in call.ResponseStream.ReadAllAsync(stoppingToken))
                {
                    // zla praktyka
                    // logger.LogInformation($"Product {update.ProductId} has {update.AvailableQuantity} items in stock.");

                    // dobra praktyka
                    logger.LogInformation("Product {ProductId} has {AvailableQuantity} items in stock.", update.ProductId, update.AvailableQuantity);

                    var stockUpdate = new
                    {
                        ProductId = update.ProductId,
                        Quantity = update.AvailableQuantity
                    };

                    await hub.Clients.All.SendAsync("StockUpdated", stockUpdate);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                logger.LogDebug(ex, "Stock stream cancelled; reconnecting.");
            }
        }
    }
}


