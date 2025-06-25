using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Content.API.Services.Models;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Domain;
using Post = Cachara.Content.API.Domain.Entities.Post;

namespace Cachara.Content.API.Services.External;

public partial class PostInternalService
{
    private readonly IPostRepository _postRepository;

    public PostInternalService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    internal static void UpdateFromInternal(Post post, PostUpsert upsert)
    {
        post.Title = upsert.Title;
        post.Body = upsert.Body;
        //TODO: Set Author


        post.ValidateAndThrow();
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
