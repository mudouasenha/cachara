using Cachara.Content.API.Infrastructure;
using Cachara.Content.API.Services;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.Controllers.Internal;

[ApiExplorerSettings(GroupName = "internal")]
[Route("internal/devtest")]
[Tags("dev")]
public class DevTestController
{
    private readonly ILogger<DevTestController> _logger;
    private readonly IPostManagerService _postManagerService;
    private readonly IServiceBusQueue _queue;


    public DevTestController(ILogger<DevTestController> logger, IPostManagerService postManagerService, IServiceBusQueue queue)
    {
        _postManagerService = postManagerService;
        _logger = logger;
        _queue = queue;
    }

    [HttpPost("test-hangfire")]
    public async Task<IResult> Enqueue()
    {
        _postManagerService.ExportPosts(Guid.NewGuid().ToString());
        return Results.Ok();
    }
    
    [HttpPost("ping")]
    public async Task<IResult> Ping()
    {
        _logger.LogInformation("Ping ok;");
        return Results.Ok();
    }

    [HttpPost("test-receive-message")]
    public async Task<IResult> TestConsumeMessage()
    {
        
        var msg = await _queue.ReceiveMessage("teste-matheus");

        if (!string.IsNullOrEmpty(msg))
        {
            return Results.Ok($"Message received: {msg}");
        }
        
        
        return Results.BadRequest("No message was received");
    }
}