using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Models;
using FluentResults;

namespace Cachara.Users.API.Services.Abstractions;

public interface IUserProfileService
{
    public Task<UserProfile> GetProfile();

    public Task<UserProfile> UpdateProfile(ProfileUpdate update);

}
