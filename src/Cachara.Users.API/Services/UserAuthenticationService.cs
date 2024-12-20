using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Models;
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

    public async Task<Result<UserRegisterResult>> RegisterUser(UserRegister register)
    {
        var result = new Result<UserRegisterResult>();
        try
        {
            var userUpsert = new UserUpsert()
            {
                FullName = register.FullName,
                UserName = register.UserName,
                Email = register.Email,
                Password = register.Password
            };
            
            var user = await base.CreateUser(userUpsert);

            var token = _tokenProvider.Generate(user);
            
            var userCreatedResult = new UserRegisterResult()
            {
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                Name = user.FullName,
                Token = token
                
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
            var user = await base.GetByEmail(request.Email);

            if (user == default)
            {
                result.WithError("Invalid userName or password");
            }

            string decryptedPassword = base.DecryptPassword(user);

            if (!string.Equals(request.Password, decryptedPassword))
            {
                result.WithError("Invalid userName or password");
            }
            
            var token = _tokenProvider.Generate(user);

            var userLoginResult = new UserLoginResult()
            {
                UserName = user.UserName,
                Name = user.FullName,
                Token = token
            };
            
            return result.WithValue(userLoginResult);
        }
        catch (Exception e)
        {
            return result.WithError(e.Message);
        }
    }

    public async Task<Result<UserProfileResult>> GetUserProfile(string token)
    {
        var profile = base.GetUserProfile();
    }
}

public class UserRegister
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserLoginResult
{
    public string Token { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
}

public class UserRegisterResult
{
    public string Token { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}