using Cachara.Data.Interfaces;
using Cachara.Domain.Commands;
using Cachara.Domain.Entities;
using Cachara.Domain.Interfaces;
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
        var post = new Post()
        {
            AuthorId = createCommand.AuthorId,
            Body = createCommand.Body,
            Title = createCommand.Title
        };

        post.GenerateId();
        post.UpdateCreatedAt();

        return await _postRepository.AddAsync(post);
    }

    public async Task<Post> GetPostById(string id)
    {
        return await _postRepository.GetByIdAsync(id) ?? throw new Exception("Post Not Found!");
    }
    
    public async Task<IEnumerable<Post>> Search(PostSearchCommand searchCommand)
    {
        return _postRepository.GetAll().ToList();
    }

    public async Task<Post> UpdatePost(PostUpdateCommand updateCommand)
    {
        var post = await _postRepository.GetByIdAsync(updateCommand.Id) ?? throw new Exception("Post Not Found");

        post.Title = updateCommand.Title;
        post.Body = updateCommand.Body;
        post.UpdateUpdateAt();

        await _postRepository.UpdateAsync(post);

        return await _postRepository.GetByIdAsync(updateCommand.Id) ?? throw new Exception("Post Not Found");
    }

    public async Task DeletePost(string id)
    {
        var post = await _postRepository.GetByIdAsync(id) ?? throw new Exception("Post Not Found");
        
        await _postRepository.DeleteAsync(post);
    }
}