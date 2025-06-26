using System.ComponentModel;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Externals;
using Cachara.Users.API.Services.Models;
using Cachara.Users.API.Services.Models.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Public;

[ApiController]
[Route("api/public/[controller]")]
[ApiExplorerSettings(GroupName = "public")]
[Tags("Auth")]
public class AuthController(UserAuthenticationService userAuthService) : ControllerBase
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">User registration data.</param>
    /// <returns>Authentication token on success.</returns>
    [HttpPost("register")]
    [EndpointSummary("Registers a new user account.")]
    [ProducesResponseType(typeof(UserRegisterResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request.UserName, request.Email, request.DateOfBirth, request.FullName, request.Password);
        var tokenResult = await userAuthService.RegisterUser(command);
        return Ok(tokenResult.Value);
    }

    /// <summary>
    /// Logs a user into the system.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication token on success.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserLoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var tokenResult = await userAuthService.LoginUser(command);
        return Ok(tokenResult.Value);
    }

    /// <summary>
    /// Logs the current user out.
    /// </summary>
    /// <returns>Logout result.</returns>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var result = await userAuthService.Logout();
        return Ok(result);
    }

    /// <summary>
    /// Changes the password for the current user.
    /// </summary>
    /// <param name="request">Current and new password.</param>
    /// <returns>True if successful.</returns>
    [HttpPut("change-password")]
    [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand { Password = request.Password, NewPassword = request.NewPassword };
        var changePasswordResult = await userAuthService.ChangePassword(command);
        return Ok(changePasswordResult.Value);
    }
}
