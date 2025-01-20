namespace Cachara.Shared.Infrastructure.AzureServiceBus;

public interface IServiceBusQueue
{
    // Send
    Task SendMessage(string queueName, string message);

    // Receive
    Task<string> ReceiveMessage(string queueName);
}
