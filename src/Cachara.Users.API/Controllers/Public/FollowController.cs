using System.Security.Claims;
using Cachara.Users.API.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers;

[ApiController]
[Route("public/[controller]")]
//[Authorize("standard-user", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiExplorerSettings(GroupName = "public")]
[Tags("Follow")]
public class FollowController(IUserFollowService userFollowService, ClaimsPrincipal user) : ControllerBase
{
    [HttpPost("{userId}")]
    public async Task<IActionResult> FollowUser(Guid userId)
    {
        await userFollowService.Follow();
        return Ok();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> UnfollowUser(Guid userId)
    {
        await userFollowService.Unfollow();
        return Ok();
    }

    [HttpGet("followers")]
    public async Task<IActionResult> GetFollowers()
    {
        await userFollowService.GetFollowers();
        return Ok();
    }

    [HttpGet("following")]
    public async Task<IActionResult> GetFollowing()
    {
        await userFollowService.GetFollowing();
        return Ok();
    }
}
