using Cachara.API.Hangfire;
using Cachara.Domain.Interfaces.Services;
using Cachara.Services.Internal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.API.Controllers.Internal;

public class DevTestController
{
    private readonly IPostManagerService _postManagerService;
    private readonly ILogger<DevTestController> _logger;

    public DevTestController(ILogger<DevTestController> logger, IPostManagerService postManagerService)
    {
        _postManagerService = postManagerService;
        _logger = logger;
    }

    [HttpPost("test-hangfire")]
    public  async Task<IResult> Enqueue()
    {
        _postManagerService.ExportPosts(Guid.NewGuid().ToString());
        return Results.Ok();
    }
}