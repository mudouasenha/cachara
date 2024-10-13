using Cachara.Content.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.Controllers.Internal;

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
}