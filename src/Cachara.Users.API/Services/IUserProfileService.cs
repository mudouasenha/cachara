using Cachara.Domain.Commands;
using Cachara.Users.API.Domain.Entities;
using FluentResults;

namespace Cachara.Users.API.Services;

public interface IUserProfileService
{
    Task<UserProfile> GetProfile();
    Task<UserProfile> UpdateProfile(ProfileUpdate update);
    Task<Result> ChangePassword(string oldPassword, string newPassword);
}