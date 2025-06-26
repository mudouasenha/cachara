using Cachara.Users.API.API.Security;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IUserProfileService = Cachara.Users.API.Services.Abstractions.IUserProfileService;

namespace Cachara.Users.API.Controllers.Public;

// TODO: Implement Rate Limiting in this API.
[ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/public/[controller]")]
[ApiExplorerSettings(GroupName = "public")]
[Tags("Account")]
public class AccountController(IUserProfileService userProfileService) : ControllerBase
{
   //private readonly IUserProfileService _userProfileService = userProfileService;

    [HttpGet("profile")]
    public async Task<UserProfile> GetProfile()
    {
        return await userProfileService.GetProfile();
    }


    [Authorize(Policies.StandardUser, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("hello-standard")]
    [EndpointDescription("This returns a hello message for a standard user or above.")]
    [EndpointSummary("Hello for standard users and above.")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public  Task<string> HelloStantard()
    {
        return Task.FromResult($"Hello Standard {HttpContext.User.Identity?.Name}");
    }

    [Authorize(Policies.ManagementUser, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("hello-management")]
    public Task<string> Hello()
    {
        return Task.FromResult($"Hello Management {HttpContext.User.Identity?.Name}");
    }

    [HttpPut("update-profile")]
    public async Task<UserProfile> UpdateProfile(ProfileUpdate update)
    {
        return await userProfileService.UpdateProfile(update);
    }
}
