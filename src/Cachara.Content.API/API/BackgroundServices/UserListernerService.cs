using Azure.Core.Extensions;
using Azure.Messaging.ServiceBus;
using Cachara.Content.API.API.Options;
using Cachara.Content.API.Services;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace Cachara.Content.API.API.BackgroundServices;

public class UserListernerService : IHostedService
{
    private const string UsersServiceBusKey = "teste-matheus";
    private readonly ILogger<UserListernerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private ServiceBusProcessor _processor;

    public UserListernerService(
        IOptions<CacharaContentOptions> options,
        ILogger<UserListernerService> logger, 
        IAzureClientFactory<ServiceBusClient> azureServiceBusFactory,
        IServiceProvider serviceProvider, 
        ServiceBusProcessor processor)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        var client = azureServiceBusFactory.CreateClient(UsersServiceBusKey);

        var usersOptions = options.Value.CacharaUsers;

        if (usersOptions.ListenerEnabled)
        {
            processor = client.CreateProcessor(
                usersOptions.ServiceBusConn,
                new ServiceBusProcessorOptions()
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = usersOptions.MaxConcurrentCalls
                }
            );

            processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        }
        
        _processor = processor;
    }

    private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        _logger.LogInformation($"Processing Message {arg.Message.MessageId}");

        using var scope = _serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        try
        {
            var @event =
                arg.Message.Body.ToString(); // TODO: Create a message body arg.Message.Body.ToObjectFromJson<DTO>();

            var userPostService = scopedServiceProvider.GetRequiredService<IPostService>();
            await userPostService.ProcessUserCreated(@event); // TODO: Actually process the event.
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await arg.CompleteMessageAsync(arg.Message, arg.CancellationToken);
        }
    }


    private async Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogInformation($"Error processing message {arg.Exception.Message}");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting {nameof(UserListernerService)}!");
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stopping {nameof(UserListernerService)}!");

        await _processor.StopProcessingAsync(cancellationToken);
    }
}
