using Cachara.Domain.Commands;
using Cachara.Domain.Entities;

namespace Cachara.Domain.Interfaces.Services;

public interface IUserService
{
    public Task<User> CreateUser(UserCreateCommand createCommand);
    public Task<User> GetUserById(string id);
    public Task<User> GetUserByUserName(string userName);
    public Task<User> GetUserPostsById(string id);
}