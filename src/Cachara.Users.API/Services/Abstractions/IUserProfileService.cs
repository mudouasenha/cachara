using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Models;
using FluentResults;

namespace Cachara.Users.API.Services.Abstractions;

public interface IUserProfileService
{
    public Task<Result<string>> Login(LoginCommand command);

    public Task<UserProfile> GetProfile();

    public Task<UserProfile> UpdateProfile(ProfileUpdate update);

    public Task<Result> ChangePassword(string oldPassword, string newPassword);
}
