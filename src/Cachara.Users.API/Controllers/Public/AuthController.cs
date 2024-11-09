using Cachara.Domain.Commands;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Public;

[ApiExplorerSettings(GroupName = "public")]
[Route("public/auth")]
[Tags("Auth")]
public class AuthController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
        public async Task<User> Register(UserUpsert upsert)
        {
            return await userService.Upsert(upsert);
        }
        
        [HttpPost("search")]
        public async Task<User> Search(string userName) // TODO: Fix Search for Users
        {
            return await userService.GetByUserName(userName);
        }

}