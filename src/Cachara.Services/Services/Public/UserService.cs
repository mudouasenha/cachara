using Cachara.Data.Interfaces;
using Cachara.Domain.Commands;
using Cachara.Domain.Entities;
using Cachara.Domain.Interfaces;
using Cachara.Domain.Interfaces.Services;

namespace Cachara.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<User> CreateUser(UserUpsert upsert)
    {
        var user = new User()
        {
            Email = upsert.Email,
            UserName = upsert.UserName,
            Password = upsert.Password,
            Name = upsert.Name
        };

        user.GenerateId();
        user.UpdateCreatedAt();

        return await _userRepository.AddAsync(user);
    }

    public async Task<User> GetByUserName(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserById(string id)
    {
        return await _userRepository.FindByAsync(p => p.Id == id) ?? throw new Exception("Post Not Found!");
    }

    public async Task<User> GetUserPostsById(string id)
    {
        throw new NotImplementedException();
    }
}