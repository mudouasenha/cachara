using Cachara.Data.Interfaces;
using Cachara.Domain.Commands;
using Cachara.Domain.Entities;
using Cachara.Domain.Interfaces.Services;

namespace Cachara.Services;


public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }
    
    public async Task<Post> CreatePost(PostCreateCommand createCommand)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> GetPostById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Post>> Search(PostSearchCommand searchCommand)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> UpdatePost(PostUpdateCommand updateCommand)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePost(string id)
    {
        throw new NotImplementedException();
    }
}