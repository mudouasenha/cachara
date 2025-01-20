using Cachara.Content.API.Domain.Commands;
using Cachara.Content.API.Domain.Entities;
using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Shared.Domain;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Models;

namespace Cachara.Content.API.Services.External;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Post> GetById(string id)
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

    public Task ProcessUserCreated(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> Upsert(PostUpsert upsert)
    {
        var expression = (Post x) => x.Id == upsert.Id;
        var entityPost = await _postRepository.FindByAsync(x => x.Id == upsert.Id);
        if (entityPost is null && upsert.Id is not null)
        {
            throw new Exception("Post not found");
        }

        entityPost = entityPost == null
            ? await InsertInternal(new Post(), user => UpdateFromInternal(user, upsert))
            : await UpdateInternal(entityPost, user => UpdateFromInternal(user, upsert));

        await _unitOfWork.Commit();
        return entityPost;
    }

    private void UpdateFromInternal(Post post, PostUpsert upsert)
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
