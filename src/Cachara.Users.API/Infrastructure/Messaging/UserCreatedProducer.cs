using System.Text.Json;
using Confluent.Kafka;

namespace Cachara.Users.API.Infrastructure.Messaging;

public class UserCreatedProducer : IAsyncDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic = "user-created";

    public UserCreatedProducer(string bootstrapServers)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishUserCreatedAsync(Guid userId, string username)
    {
        var userCreated = new
        {
            UserId = userId,
            Username = username,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var message = new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(userCreated)
        };

        await _producer.ProduceAsync(_topic, message);

        Console.WriteLine("UserCreated event published!");
    }

    public async ValueTask DisposeAsync()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
        await Task.CompletedTask;
    }
}
