using System.Text.Json;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Cachara.Users.API.Controllers.Internal;

[ApiExplorerSettings(GroupName = "internal")]
[Route("internal/devtest")]
[Tags("Dev")]
public class DevTestController
{
    private readonly ILogger<DevTestController> _logger;
    private readonly IServiceBusQueue _queue;

    public DevTestController(ILogger<DevTestController> logger, IServiceBusQueue queue)
    {
        _queue = queue;
        _logger = logger;
    }
    
    [HttpPost("ping")]
    [EndpointSummary("Ping the application.")]
    [EndpointDescription("Makes sure that the request is received and returned by only returning an Ok Result.")]
    public async Task<IResult> Ping()
    {
        _logger.LogInformation("Ping ok;");
        return Results.Ok();
    }
    
    [HttpPost("test-exception-logging")]
    public async Task<IResult> TestException()
    {
        var ex = new ValidationException("TestValidationException");
        _logger.LogError(ex, "Exception found: {Message} {@Errors} {@Exception};", ex.Message, ex.Errors, ex);
        _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
        return Results.Ok();
    }

    [HttpPost("test-send-message")]
    public async Task<IResult> TestSendMessage()
    {
        await _queue.SendMessage("teste-matheus", "Message from Users API");
        return Results.Ok("Message Sent");
    }
    
    [HttpPost("test-memory-cache")]
    public async Task<IResult> TestMemoryCache([FromServices] IMemoryCache memoryCache)
    {
        if (!memoryCache.TryGetValue("teste-memory-cache", out IEnumerable<string> userNames))
        {
            userNames = new[] { "maria", "joão" };
            memoryCache.Set("teste-memory-cache", userNames, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            });
        }

        return Results.Ok(userNames);
    }
    
    [HttpPost("test-distributed-cache")]
    public async Task<IResult> TestMemoryCache([FromServices] IDistributedCache distributedCache)
    {
        var userNamesString = await distributedCache.GetStringAsync("teste-distributed-cache");

        if (!string.IsNullOrWhiteSpace(userNamesString))
            return Results.Ok(JsonSerializer.Deserialize<IEnumerable<string>>(userNamesString));
        
        var userNames = new[] { "maria", "joão" };

        await distributedCache.SetStringAsync("teste-distributed-cache",
            JsonSerializer.Serialize(userNames),
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            });

        return Results.Ok(userNames);
    }
    
    
}