using Cachara.Data.Interfaces;
using Cachara.Domain.Commands;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Domain.Errors;
using Cachara.Users.API.Domain.Specification;
using Cachara.Users.API.Services.Abstractions;
using FluentResults;

namespace Cachara.Users.API.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserProfileService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }
    public async Task<Result<string>> Login(LoginCommand command)
    {
        var result = new Result<string>();
        var specification = new UserByEmailSpecification(command.Email);
        var user = await _userRepository.FindByAsync(specification.ToExpression());
        if (user is null)
        {
            return result.WithError(DomainErrors.User.InvalidCredentials);
        }

        string token = _jwtProvider.Generate(user);
        
        
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