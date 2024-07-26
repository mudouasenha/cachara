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
    public async Task<User> CreateUser(UserCreateCommand createCommand)
    {
        var user = new User()
        {
            Email = createCommand.Email,
            UserName = createCommand.UserName,
            Password = createCommand.Password,
            Name = createCommand.Name
        };

        user.GenerateId();
        user.UpdateCreatedAt();

        return await _userRepository.AddAsync(user);
    }

    public async Task<User> GetUserByUserName(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserById(string id)
    {
        return await _userRepository.GetByIdAsync(id) ?? throw new Exception("Post Not Found!");
    }

    public async Task<User> GetUserPostsById(string id)
    {
        throw new NotImplementedException();
    }
}