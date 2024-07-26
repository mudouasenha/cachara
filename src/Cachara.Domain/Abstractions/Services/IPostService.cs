using Cachara.Domain.Commands;
using Cachara.Domain.Entities;

namespace Cachara.Domain.Interfaces.Services;

public interface IPostService
{
    public Task<Post> CreatePost(PostCreateCommand createCommand);
    public Task<Post> GetPostById(string id);
    public Task<IEnumerable<Post>> Search(PostSearchCommand searchCommand);
    public Task<Post> UpdatePost(PostUpdateCommand updateCommand);
    public Task DeletePost(string id);
}