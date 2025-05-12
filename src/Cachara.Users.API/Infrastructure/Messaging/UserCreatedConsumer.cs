using System.Text.Json;
using Confluent.Kafka;

namespace Cachara.Users.API.Services;

public class UserCreatedConsumer : IDisposable
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic = "user-created";

    public UserCreatedConsumer(string bootstrapServers)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "user-service-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(_topic);
    }

    public void StartConsuming(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(cancellationToken);
                Console.WriteLine($"Consumed: {result.Message.Value}");
            }
        }
        catch (OperationCanceledException)
        {
            // Shutdown signal
        }
        finally
        {
            _consumer.Close();
        }
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}


