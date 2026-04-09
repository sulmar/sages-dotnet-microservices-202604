using StackExchange.Redis;

namespace Payment.Api;

public class PaymentWorker(IConnectionMultiplexer redis) : BackgroundService
{
    private const string StreamKey = "orders-stream";
    private const string ConsumerGroupName = "payment-group";

    private IDatabase db => redis.GetDatabase();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await EnsureGroup();

        var consumer = Environment.GetEnvironmentVariable("INSTANCE_ID"); // Use instance id as consumer name

        while (!stoppingToken.IsCancellationRequested)
        {
            // XREADGROUP GROUP payment-group consumer-1 COUNT 1 STREAMS orders-stream >
            var messages = await db.StreamReadGroupAsync(StreamKey, ConsumerGroupName, consumer, ">", count: 1);

            foreach (var message in messages)
            {
                await HandleOrder(message, consumer);
            }

            await Task.Delay(5000, stoppingToken); // Poll every second
        }
    }

    private async Task HandleOrder(StreamEntry message, string consumer)
    {
        var values = message.Values.ToDictionary(x=> x.Name, x=> x.Value.ToString());

        var type = values["type"];
        var orderId = values["orderId"];
        var amount = values["amount"];
        var status = values["status"];

        Console.WriteLine($"Processing order {orderId} of type {type} with amount {amount} and status {status} by consumer {consumer}...");

        await Task.Delay(2000); // Simulate payment processing delay

        Console.WriteLine("Done.");

        // XACK orders-stream payment-group messageId
        await db.StreamAcknowledgeAsync(StreamKey, ConsumerGroupName, message.Id);
    }

    private async Task EnsureGroup()
    {
        var groups = await db.StreamGroupInfoAsync(StreamKey);

        if (!groups.Any(g => g.Name == ConsumerGroupName))
            //  XGROUP CREATE orders-stream payment-group $ MKSTREAM [MKSTREAM
            await db.StreamCreateConsumerGroupAsync(StreamKey, ConsumerGroupName, "$", createStream: true);
    }
}
