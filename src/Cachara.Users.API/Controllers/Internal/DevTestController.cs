using FluentValidation;
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
    
    [HttpPost("test-exception-logging")]
    public async Task<IResult> TestException()
    {
        var ex = new ValidationException("TestValidationException");
        _logger.LogError(ex, "Exception found: {Message} {@Errors} {@Exception};", ex.Message, ex.Errors, ex);
        _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
        return Results.Ok();
    }
}