using Cachara.Domain.Commands;
using Cachara.Domain.Entities;

namespace Cachara.Domain.Interfaces.Services;

public interface IPostService
{
    public Task<Post> GetById(string id);
    public Task<IEnumerable<Post>> Search(PostSearchCommand searchCommand);
    public Task<Post> Upsert(PostUpsert upsert);
    public Task Delete(string id);
}