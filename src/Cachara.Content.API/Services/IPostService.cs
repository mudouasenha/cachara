using Cachara.Content.API.Domain.Entities;
using Cachara.Domain.Commands;

namespace Cachara.Content.API.Services;

public interface IPostService
{
    public Task<Post> GetById(string id);
    public Task<IEnumerable<Post>> Search(PostSearchCommand searchCommand);
    public Task<Post> Upsert(PostUpsert upsert);
    public Task Delete(string id);
}