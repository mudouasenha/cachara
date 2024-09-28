using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cachara.API.HealthChecks;

// TODO: HealthCheck
public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public RabbitMQHealthCheck(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var factory = new ConnectionFactory { Uri = new Uri(_connectionString) };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            // Optionally, you can check for a specific queue
            channel.QueueDeclarePassive("your_queue_name"); // Replace with your queue name

            return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ is healthy."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ is unhealthy.", ex));
        }
    }
}