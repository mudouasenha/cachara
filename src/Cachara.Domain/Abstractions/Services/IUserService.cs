using Cachara.Domain.Commands;
using Cachara.Domain.Entities;

namespace Cachara.Domain.Interfaces.Services;

public interface IUserService
{
    public Task<User> Upsert(UserUpsert upsert);
    public Task<IEnumerable<User>> Search(UserSearchCommmand searchCommmand);
    public Task<User> GetUserById(string id);
    public Task<User> GetByUserName(string userName);
    public Task<User> GetWithPostsById(string id);
}