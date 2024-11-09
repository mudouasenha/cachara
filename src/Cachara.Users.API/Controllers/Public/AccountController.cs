using Cachara.Domain.Commands;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Public;


[ApiExplorerSettings(GroupName = "public")]
[Route("public/account")]
[Tags("Auth")]
public class AccountController(IUserProfileService userProfileService) : ControllerBase
{
        [HttpGet("profile")]
        public async Task<UserProfile> GetProfile()
        {
            return await userProfileService.GetProfile();
        }
            
        [Authorize("standard-user")]
        [HttpGet("hello-standard")]
        public async Task<string> HelloStantard()
        {
            return $"Hello Standard {HttpContext.User.Identity?.Name}";
        }
        
        [Authorize("management-user")]
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
        
        [HttpPut("change-password")]
        public async Task<Result> ChangePassword(string oldPassword, string newPassword)
        {
            return await userProfileService.ChangePassword(oldPassword, newPassword);
        }

}