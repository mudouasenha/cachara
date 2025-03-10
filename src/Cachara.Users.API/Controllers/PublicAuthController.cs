using Cachara.Users.API.Controllers.Base;
using Cachara.Users.API.Services;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Public;

[ApiExplorerSettings(GroupName = "public")]
[Route("public/auth")]
[Tags("Auth")]
public class PublicAuthController(IUserService userService, UserAuthenticationService userAuthService) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.UserName, request.Email, request.DateOfBirth, request.FullName,
            request.Password);
        var tokenResult = await userAuthService.RegisterUser(command);

        if (tokenResult.IsFailed)
        {
            return HandleFailure(); // TODO: Implement failed result factory. 400
        }

        return Ok(tokenResult.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var tokenResult = await userAuthService.LoginUser(command);

        if (tokenResult.IsFailed)
        {
            return HandleFailure(); // TODO: Implement failed result factory. 400
        }

        return Ok(tokenResult.Value);
    }
}
