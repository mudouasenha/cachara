using Cachara.Domain.Commands;
using Cachara.Users.API.Domain.Entities;

namespace Cachara.Users.API.Services.Abstractions;

public interface IUserService
{
    public Task<User> Upsert(UserUpsert upsert);
    public Task<IEnumerable<User>> Search(UserSearchCommand searchCommmand);
    public Task<User> GetUserById(string id);
    public Task<User> GetByUserName(string userName);
    public Task<User> GetWithPostsById(string id);
}