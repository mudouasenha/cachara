using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Domain.Errors;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Models;
using Cachara.Users.API.Services.Models.Internal;
using FluentResults;

namespace Cachara.Users.API.Services;

public class UserAuthenticationService : UserService
{
    private readonly IJwtProvider _tokenProvider;

    public UserAuthenticationService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtProvider tokenProvider)
        : base(userRepository, unitOfWork)
    {
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<UserRegisterResult>> RegisterUser(RegisterCommand register)
    {
        var result = new Result<UserRegisterResult>();
        try
        {
            var userUpsert = new UserUpsert
            {
                FullName = register.FullName,
                UserName = register.UserName,
                Email = register.Email,
                Password = register.Password,
                DateOfBirth = register.DateOfBirth,
                Subscription = Subscription.Standard
            };

            var user = await CreateUser(userUpsert);

            var token = _tokenProvider.Generate(user);

            var userCreatedResult = new UserRegisterResult
            {
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                Name = user.FullName,
                Token = token,
            };

            return result.WithValue(userCreatedResult);
        }
        catch (Exception e)
        {
            return result.WithError(e.Message);
        }
    }

    public async Task<Result<UserLoginResult>> LoginUser(LoginCommand request)
    {
        var result = new Result<UserLoginResult>();
        try
        {
            var user = await GetByEmail(request.Email);

            if (user == default)
            {
                result.WithError(DomainErrors.UserAuthentication.InvalidCredentials);
            }

            var decryptedPassword = DecryptPassword(user);

            if (!string.Equals(request.Password, decryptedPassword))
            {
                result.WithError(DomainErrors.UserAuthentication.InvalidCredentials);
            }

            var token = _tokenProvider.Generate(user);

            var userLoginResult = new UserLoginResult
            {
                UserName = user.UserName,
                Name = user.FullName,
                Token = token,
            };

            return result.WithValue(userLoginResult);
        }
        catch (Exception e)
        {
            return result.WithError(e.Message);
        }
    }

    public async Task<Result<ChangePasswordResult>> ChangePassword(string oldPassword, string newPassword)
    {
        var result = new Result<ChangePasswordResult>();

        // Step 1: Retrieve the user from the token
        var user = await GetUserFromTokenAsync();
        if (user == null)
        {
            return result.WithError(DomainErrors.UserAuthentication.UserNotFound);
        }

        // Step 2: Verify old password
        var isOldPasswordValid = _passwordHasher.Verify(user.HashedPassword, oldPassword);
        if (!isOldPasswordValid)
        {
            return result.WithError(DomainErrors.UserAuthentication.InvalidCredentials);
        }

        // Step 3: Ensure the new password meets security requirements
        if (!IsValidPassword(newPassword))
        {
            return result.WithError(DomainErrors.UserAuthentication.UnsafePassword);
        }

        // Step 4: Prevent reusing the old password
        if (_passwordHasher.Verify(user.HashedPassword, newPassword))
        {
            return result.WithError(DomainErrors.UserAuthentication.SamePassword);
        }

        // Step 5: Hash and update the password
        user.HashedPassword = _passwordHasher.Hash(newPassword);

        // Step 6: Update user data in the database
        await _userRepository.UpdateAsync(user);

        // Step 7: Revoke active sessions or refresh tokens (recommended)
        await _sessionService.RevokeUserSessionsAsync(user.Id);


        return new ChangePasswordResult
        {
            Message = "Password changed successfully", LastChanged = DateTimeOffset.UtcNow
        };
    }

    // Example password validation logic
    private bool IsValidPassword(string password)
    {
        return password.Length >= 8
               && password.Any(char.IsUpper)
               && password.Any(char.IsLower)
               && password.Any(char.IsDigit)
               && password.Any(ch => !char.IsLetterOrDigit(ch)); // Symbol check
    }
}

public class ChangePasswordResult
{
    public string Message { get; set; }
    public DateTimeOffset LastChanged { get; set; } // TODO: Consider offset on result
}

public class UserLoginResult
{
    public TokenResult Token { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
}

public class UserRegisterResult
{
    public TokenResult Token { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
