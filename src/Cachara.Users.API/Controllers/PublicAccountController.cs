using Cachara.Users.API.API.Security;
using Cachara.Users.API.Controllers.Base;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Models;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IUserProfileService = Cachara.Users.API.Services.Abstractions.IUserProfileService;

namespace Cachara.Users.API.Controllers.Public;

// TODO: Implement Rate Limiting in this API.

[ApiExplorerSettings(GroupName = "public")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("public/account")]
[Tags("Auth")]
public class PublicAccountController(IUserProfileService userProfileService) : BaseController
{
    private readonly IUserProfileService _userProfileService = userProfileService;

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
    public async Task<string> HelloStantard()
    {
        return $"Hello Standard {HttpContext.User.Identity?.Name}";
    }

    [Authorize(Policies.ManagementUser, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("hello-management")]
    public async Task<string> Hello()
    {
        return $"Hello Management {HttpContext.User.Identity?.Name}";
    }

    [HttpPut("update-profile")]
    public async Task<UserProfile> UpdateProfile(ProfileUpdate update)
    {
        return await userProfileService.UpdateProfile(update);
    }
}
