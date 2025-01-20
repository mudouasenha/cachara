using Azure.Messaging.ServiceBus;
using Cachara.Content.API.API.Options;
using Cachara.Content.API.Services;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace Cachara.Content.API.API.BackgroundServices;

public class UserListernerService : IHostedService
{
    public const string UsersServiceBusKey = "teste-matheus";
    private readonly ILogger<UserListernerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusProcessor _processor;

    public UserListernerService(
        IOptions<CacharaContentOptions> options,
        ILogger<UserListernerService> logger,
        IAzureClientFactory<ServiceBusClient> azureServiceBusFactory,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        var client = azureServiceBusFactory.CreateClient(UsersServiceBusKey);

        var usersOptions = options.Value.CacharaUsers;

        if (usersOptions.ListenerEnabled)
        {
            _processor = client.CreateProcessor(
                UsersServiceBusKey,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false, MaxConcurrentCalls = usersOptions.MaxConcurrentCalls
                }
            );

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        }
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
}
