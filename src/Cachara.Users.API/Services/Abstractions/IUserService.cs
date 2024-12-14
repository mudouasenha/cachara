using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Models;

namespace Cachara.Users.API.Services.Abstractions;

public interface IUserService
{
    public Task<User> Upsert(UserUpsert upsert);
    public Task<IEnumerable<User>> Search(UserSearchCommand searchCommmand);
    public Task<User> GetUserById(string id);
    public Task<User> GetByUserName(string userName);
    public Task<User> GetWithPostsById(string id);
}