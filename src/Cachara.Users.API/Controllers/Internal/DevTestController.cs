using System.Text.Json;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace Cachara.Users.API.Controllers.Internal;

[ApiController]
[Route("internal/[controller]")]
[ApiExplorerSettings(GroupName = "internal")]
[Tags("Dev")]
public class DevTestController : ControllerBase
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

    [HttpPost("test-send-message")]
    public async Task<IResult> TestSendMessage()
    {
        await _queue.SendMessage("teste-matheus", "Message from Users API");
        return Results.Ok("Message Sent");
    }

    [HttpPost("test-memory-cache")]
    public Task<IResult> TestMemoryCache([FromServices] IMemoryCache memoryCache)
    {
        if (!memoryCache.TryGetValue("teste-memory-cache", out IEnumerable<string> userNames))
        {
            userNames = new[] { "maria", "joão" };
            memoryCache.Set("teste-memory-cache", userNames,
                new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) });
        }

        return Task.FromResult(Results.Ok(userNames));
    }

    [HttpPost("test-distributed-cache")]
    public async Task<IResult> TestDistributedCache([FromServices] IDistributedCache distributedCache)
    {
        var userNamesString = await distributedCache.GetStringAsync("teste-distributed-cache");

        if (!string.IsNullOrWhiteSpace(userNamesString))
        {
            return Results.Ok(JsonSerializer.Deserialize<IEnumerable<string>>(userNamesString));
        }

        var userNames = new[] { "maria", "joão" };

        await distributedCache.SetStringAsync("teste-distributed-cache",
            JsonSerializer.Serialize(userNames),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) });

        return Results.Ok(userNames);
    }

    [HttpPost("test-hybrid-cache-getorcreateasync")]
    public async Task<IResult> GetHybridCache([FromServices] HybridCache hybridCache)
    {
        var result = await hybridCache.GetOrCreateAsync("test-hybrid-cache-getorcreateasync",
            cancellationToken => ValueTask.FromResult(new[] { "maria", "joão" }),
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(10), LocalCacheExpiration = TimeSpan.FromSeconds(10)
                //Flags = HybridCacheEntryFlags.DisableDistributedCache  AND OTHER OPTIONS
            },
            new[] { "tag1", "tag2" });

        return Results.Ok(result);
    }

    [HttpPost("test-hybrid-cache-set")]
    public async Task<IResult> SetHybridCache([FromServices] HybridCache hybridCache)
    {
        await hybridCache.SetAsync("test-hybrid-cache-set",
            new[] { "maria", "joão" },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(10), LocalCacheExpiration = TimeSpan.FromSeconds(10)
                //Flags = HybridCacheEntryFlags.DisableDistributedCache  AND OTHER OPTIONS
            },
            new[] { "tag1", "tag2" });

        return Results.Ok();
    }

    [HttpPost("test-hybrid-cache-remove")]
    public async Task<IResult> RemoveHybridCache([FromServices] HybridCache hybridCache)
    {
        await hybridCache.RemoveAsync("test-hybrid-cache-getorcreateasync");

        // REMOVE BY TAG
        // await hybridCache.RemoveByTagAsync("tag1");

        return Results.Ok();
    }
}
