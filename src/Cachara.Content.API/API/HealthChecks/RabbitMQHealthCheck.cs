using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Cachara.Content.API.API.HealthChecks;

// TODO: HealthCheck
public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public RabbitMQHealthCheck(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        try
        {
            var factory = new ConnectionFactory { Uri = new Uri(_connectionString) };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            // Optionally, you can check for a specific queue
            await channel.QueueDeclarePassiveAsync("your_queue_name"); // Replace with your queue name

            return HealthCheckResult.Healthy("RabbitMQ is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is unhealthy.", ex);
        }
    }
}
