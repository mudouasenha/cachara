using Cachara.Shared.Application.Abstractions;
using Cachara.Shared.Application.Errors;
using Cachara.Shared.Domain;
using Cachara.Shared.Domain.Entities;
using Cachara.Shared.Infrastructure;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.Domain.Errors;
using Cachara.Users.API.Infrastructure;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Infrastructure.Security;
using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Models;
using Cachara.Users.API.Services.Models.Internal;
using Cachara.Users.API.Services.Validations;
using FluentResults;
using MapsterMapper;

namespace Cachara.Users.API.Services.Externals;

public class UserAuthenticationService : UserService
{
    private readonly IMapper _mapper;
    private readonly ILogger<UserAuthenticationService> _logger;
    private readonly IJwtProvider _tokenProvider;
    private readonly ISessionStoreService<UserAccount> _sessionStore;
    private readonly IAccountService<UserAccount> _userAccountService;
    private readonly IAggregateExceptionHandler _aggregateExceptionHandler;


    public UserAuthenticationService(
        IMapper mapper,
        ILogger<UserAuthenticationService> logger,
        IAggregateExceptionHandler aggregateExceptionHandler,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtProvider tokenProvider,
        IGeneralDataProtectionService generalDataProtectionService,
        IAccountService<UserAccount> userAccountService,
        ISessionStoreService<UserAccount> sessionStore)
        : base(mapper, userRepository, generalDataProtectionService, unitOfWork)
    {
        _mapper = mapper;
        _logger = logger;
        _aggregateExceptionHandler = aggregateExceptionHandler;
        _tokenProvider = tokenProvider;
        _userAccountService = userAccountService;
        _sessionStore = sessionStore;
    }

    public async Task<Result<UserRegisterResult>> RegisterUser(RegisterCommand register)
    {
        var result = new Result<UserRegisterResult>();
        try
        {
            var userUserNameExists = await FindByUserName(register.UserName);
            if (userUserNameExists != null)
            {
                return Result.Fail(DomainErrors.UserAuthentication.UserNameAlreadyExists);
            }
            var userEmailExists = await GetByEmail(register.Email);
            if (userEmailExists != null)
            {
                return Result.Fail(DomainErrors.UserAuthentication.UserEmailAlreadyExists);
            }

            var userUpsert = new UserUpsert
            {
                FullName = register.FullName,
                UserName = register.UserName,
                Email = register.Email,
                Password = register.Password,
                DateOfBirth = register.DateOfBirth,
                Subscription = Subscription.Standard
            };

            var userCreateResult = await CreateUser(userUpsert);

            if (userCreateResult.IsFailed)
            {
                return Result.Fail(DomainErrors.User.CreateUserFailed);
            }

            var user = await FindById(userCreateResult.Value.Id);

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
        try
        {
            var user = await FindByEmail(request.Email);
            if (user is null)
            {
                return Result.Fail(DomainErrors.UserAuthentication.InvalidCredentials);
            }

            var decryptedPassword = DecryptPassword(user);

            if (!string.Equals(request.Password, decryptedPassword))
            {
                return Result.Fail(DomainErrors.UserAuthentication.InvalidCredentials);
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

            return Result.Ok(userLoginResult);
        }
        catch (Exception ex)
        {
            return _aggregateExceptionHandler.Handle(ex);
        }
    }

    public async Task<Result> Logout()
    {
        try
        {
            var userAccount = _userAccountService.Current;
            await _sessionStore.InvalidateAllSessionsAsync(userAccount.Id);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return _aggregateExceptionHandler.Handle(ex);
        }

    }


    public async Task<Result<ChangePasswordResult>> ChangePassword(ChangePasswordCommand command)
    {
        try
        {
            var userAccount = _userAccountService.Current;
            var user = await FindById(userAccount.Id);
            if (user == null)
            {
                return Result.Fail(DomainErrors.UserAuthentication.UserNotFound);
            }

            var isOldPasswordValid = VerifyPassword(user, command.Password);
            if (!isOldPasswordValid)
            {
                return Result.Fail(DomainErrors.UserAuthentication.InvalidCredentials);
            }

            var passwordValidationResult = new ChangePasswordCommandValidator().Validate(command);
            if (!passwordValidationResult.IsValid)
            {
                return new Result<ChangePasswordResult>().WithErrorsFromValidationResult(passwordValidationResult);
            }

            if (VerifyPassword(user, command.NewPassword))
            {
                return Result.Fail(DomainErrors.UserAuthentication.SamePassword);
            }

            // Step 5: Hash and update the password
            await UpdatePassword(user, command.NewPassword);

            // Step 7: Revoke active sessions or refresh tokens (recommended)
            await _sessionStore.InvalidateAllSessionsAsync(user.Id);

            return new ChangePasswordResult
            {
                Message = "Password changed successfully, please log in again.", LastChanged = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex)
        {
            return _aggregateExceptionHandler.Handle(ex);
        }

    }
}
