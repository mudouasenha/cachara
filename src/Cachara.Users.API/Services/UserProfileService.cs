using Cachara.Domain.Commands;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Abstractions;
using FluentResults;

namespace Cachara.Users.API.Services;

public class UserProfileService : IUserProfileService
{
    public Task<string> Login()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfile> GetProfile()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfile> UpdateProfile(ProfileUpdate update)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ChangePassword(string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }
}