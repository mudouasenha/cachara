using Cachara.Users.API.Controllers.Base;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Models;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Public;

[ApiExplorerSettings(GroupName = "public")]
[Route("public/auth")]
[Tags("Auth")]
public class AuthController(IUserService userService, UserAuthenticationService userAuthService) : BaseController
{
    
    [HttpPost("register")]
        public async Task<User> Register(UserUpsert upsert)
        {
            return await userService.Upsert(upsert);
        }
        
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email);
        Result<string> tokenResult = await userAuthService.LoginUser(request);

        if (tokenResult.IsFailed)
        {
            return HandleFailure(); // TODO: Implement failed result factory. 400
        }

        return Ok(tokenResult.Value);
    }
        
        [HttpPost("search")]
        public async Task<User> Search(string userName) // TODO: Fix Search for Users
        {
            return await userService.GetByUserName(userName);
        }

}