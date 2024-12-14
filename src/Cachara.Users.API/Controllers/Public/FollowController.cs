using System.Security.Claims;
using Cachara.Users.API.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Public;

[Authorize()]
[ApiExplorerSettings(GroupName = "public")]
[Route("public/follow")]
[Tags("Auth")]
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