using Cachara.Shared.Application;
using Cachara.Shared.Application.Errors;
using Cachara.Shared.Domain;
using Cachara.Shared.Infrastructure;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Domain.Errors;
using Cachara.Users.API.Infrastructure.Cache;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Errors;
using Cachara.Users.API.Services.Models;
using Cachara.Users.API.Services.Models.Internal;
using Cachara.Users.API.Services.Validations;
using FluentResults;
using Microsoft.Extensions.Caching.Distributed;

namespace Cachara.Users.API.Services;

public class UserAuthenticationService : UserService
{
    private readonly ILogger<UserAuthenticationService> _logger;
    private readonly IJwtProvider _tokenProvider;
    private readonly ICacheService _cache;
    private readonly ISessionStoreService<UserAccount> _sessionStore;
    private readonly IAccountService<UserAccount> _userAccountService;
    private readonly IAggregateExceptionHandler _aggregateExceptionHandler;


    public UserAuthenticationService(
        ILogger<UserAuthenticationService> logger,
        IAggregateExceptionHandler aggregateExceptionHandler,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtProvider tokenProvider,
        IGeneralDataProtectionService generalDataProtectionService,
        IAccountService<UserAccount> userAccountService,
        ICacheService cache)
        : base(userRepository, generalDataProtectionService, unitOfWork)
    {
        _logger = logger;
        _aggregateExceptionHandler = aggregateExceptionHandler;
        _tokenProvider = tokenProvider;
        _userAccountService = userAccountService;
        _cache = cache;
    }

    public async Task<Result<UserRegisterResult>> RegisterUser(RegisterCommand register)
    {
        _logger.LogInformation("Register started — Seq test log");
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

            var account = _tokenProvider.GetAccount(token.Token);

            var session = await _sessionStore.CreateSession(account);

            var userCreatedResult = new UserRegisterResult
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.FullName,
                Token = token,
                SessionId = session.Id
            };

            return result.WithValue(userCreatedResult);
        }
        catch (Exception ex)
        {
            return _aggregateExceptionHandler.Handle(ex);
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
                result.WithError(ApplicationErrors.UserAuthentication.InvalidCredentials);
            }

            var decryptedPassword = DecryptPassword(user);

            if (!string.Equals(request.Password, decryptedPassword))
            {
                result.WithError(ApplicationErrors.UserAuthentication.InvalidCredentials);
            }

            var token = _tokenProvider.Generate(user);

            var account = _tokenProvider.GetAccount(token.Token);

            var session = await _sessionStore.CreateSession(account);

            var userLoginResult = new UserLoginResult
            {
                UserName = user.UserName,
                Name = user.FullName,
                Token = token,
                SessionId = session.Id
            };

            return result.WithValue(userLoginResult);
        }
        catch (Exception ex)
        {
            return _aggregateExceptionHandler.Handle(ex);
        }
    }

    public async Task<Result> Logout(string sessionId)
    {
        throw new NotImplementedException();
        //await _userAccountService.InvalidateSession(sessionId);
        //return Result.Success();
    }

    public async Task<Result<ChangePasswordResult>> ChangePassword(ChangePasswordCommand command)
    {
        var result = new Result<ChangePasswordResult>();

        // Step 1: Retrieve the user from the token
        var userAccount = _userAccountService.Current;
        var user = await GetUserById(userAccount.Id);
        if (user == null)
        {
            return result.WithError(ApplicationErrors.UserAuthentication.UserNotFound);
        }

        var isOldPasswordValid = VerifyPassword(user, command.Password);
        if (!isOldPasswordValid)
        {
            return result.WithError(ApplicationErrors.UserAuthentication.InvalidCredentials);
        }

        var passwordValidationResult = new ChangePasswordCommandValidator().Validate(command);
        if (!passwordValidationResult.IsValid)
        {
            return result.WithErrorsFromValidationResult(passwordValidationResult);
        }

        if (VerifyPassword(user, command.NewPassword))
        {
            return result.WithError(ApplicationErrors.UserAuthentication.SamePassword);
        }

        // Step 5: Hash and update the password
        await UpdateInternal(user, userUpdate => userUpdate.Password = command.NewPassword);

        // Step 7: Revoke active sessions or refresh tokens (recommended)
        //await _sessionService.RevokeUserSessionsAsync(user.Id);

        return new ChangePasswordResult
        {
            Message = "Password changed successfully", LastChanged = DateTimeOffset.UtcNow
        };
    }
}
