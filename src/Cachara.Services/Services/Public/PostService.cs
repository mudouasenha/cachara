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
    
    public async Task<Post> Upsert(PostUpsert upsert)
    {
        var expression = (Post x) => x.Id == upsert.Id;
        var post = _postRepository.FindByAsync(x => x.Id )
        if (entityUser is null && userSpecification.HasIdIdentifier())
        {
            throw new DomainException("User not found");
        }

        entityUser = entityUser == null ?
            await InsertInternal(new User(), (user) => UpdateFromInternal(user, upsert))
            :
            await UpdateInternal(entityUser, (user) => UpdateFromInternal(user, upsert));

        await unitOfWork.Commit();
        
    }

    private void UpdateFromInternal(Post user, PostUpsert upsert)
    {
        user.Title = upsert.Title;
        user.Body = upsert.Body;
        //TODO: Set Author
    }

    public async Task<Post> GetPostById(string id)
    {
        return await _postRepository.FindByAsync(x => x.Id == id) ?? throw new Exception("Post Not Found!");
    }

    public async Task<IEnumerable<Post>> Search(PostSearchCommand searchCommand)
    {
        return _postRepository.GetEntities().ToList();
    }

    public async Task Delete(string id)
    {
        var post = await _postRepository.FindByAsync(x => x.Id == id) ?? throw new Exception("Post Not Found");
        
        await _postRepository.RemoveAsync(post);
    }
    
    internal async Task<Post> InsertInternal(
        Post post,
        Action<Post> entityUpdate = null
    )
    {
        post.GenerateId();
        post.UpdateCreatedAt();
        

        entityUpdate?.Invoke(post);

        await _postRepository.AddAsync(post);
        return post;
    }

    internal async Task<Post> UpdateInternal(
        Post post,
        Action<Post> entityUpdate = null
    )
    {
        post.UpdatedAt = DateTimeOffset.UtcNow;

        entityUpdate?.Invoke(post);

        await _postRepository.EditAsync(post);
        return post;
    }
}