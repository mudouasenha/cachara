using Cachara.Content.API.Infrastructure;
using Cachara.Content.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.Controllers.Internal;

[ApiExplorerSettings(GroupName = "internal")]
[Route("internal/devtest")]
[Tags("dev")]
public class DevTestController
{
    private readonly ILogger<DevTestController> _logger;
    private readonly IPostManagerService _postManagerService;

    public DevTestController(ILogger<DevTestController> logger, IPostManagerService postManagerService)
    {
        _postManagerService = postManagerService;
        _logger = logger;
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
}