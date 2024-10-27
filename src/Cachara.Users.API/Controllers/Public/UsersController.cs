using Cachara.Domain.Commands;
using Cachara.Domain.Interfaces.Services;
using Cachara.Users.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Public;

[ApiExplorerSettings(GroupName = "internal")]
[Route("public/users")]
[Tags("Posts")]
public class UsersController(IUserService userService) : ControllerBase
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