using System.Threading.Channels;

namespace Ordering.Api.Workers;

public class OrderWorker(Channel<Ordering.Domain.Order> channel, OrderProcessor processor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach(var order in channel.Reader.ReadAllAsync(stoppingToken))
        {
           await processor.ProcessAsync(order);
        }
    }
}
