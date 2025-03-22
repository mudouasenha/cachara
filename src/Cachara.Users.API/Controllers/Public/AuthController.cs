using Cachara.Users.API.Services;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers;

[ApiController]
[Route("public/[controller]")]
[ApiExplorerSettings(GroupName = "public")]
[Tags("Auth")]
public class AuthController(IUserService userService, UserAuthenticationService userAuthService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request.UserName, request.Email, request.DateOfBirth, request.FullName,
            request.Password);
        var tokenResult = await userAuthService.RegisterUser(command);

        // if (tokenResult.IsFailed)
        // {
        //     return HandleFailure(); // TODO: Implement failed result factory. 400
        // }

        return Ok(tokenResult.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var tokenResult = await userAuthService.LoginUser(command);

        // if (tokenResult.IsFailed)
        // {
        //     return HandleFailure(); // TODO: Implement failed result factory. 400
        // }

        return Ok(tokenResult.Value);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
    {
        var command = new ChangePasswordCommand() { Password = oldPassword, NewPassword = newPassword, };
        var changePasswordResult = await userAuthService.ChangePassword(command);

        // if (changePasswordResult.IsFailed)
        // {
        //     return HandleFailure(); // TODO: Implement failed result factory. 400
        // }

        return Ok(changePasswordResult.Value);
    }
}
