using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Internal;

[ApiExplorerSettings(GroupName = "internal")]
[Route("internal/devtest")]
[Tags("Dev")]
public class DevTestController
{
    private readonly ILogger<DevTestController> _logger;

    public DevTestController(ILogger<DevTestController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("ping")]
    public async Task<IResult> Ping()
    {
        _logger.LogInformation("Ping ok;");
        return Results.Ok();
    }
}