using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Infrastructure;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Infrastructure.Security;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Models;
using FluentResults;

namespace Cachara.Users.API.Services.Externals;

public class UserProfileService : IUserProfileService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserRepository _userRepository;

    public UserProfileService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
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
