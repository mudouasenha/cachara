using Cachara.Content.API.Domain.Commands;
using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Content.API.Services.Models;
using Cachara.Shared.Application.Errors;
using Cachara.Shared.Application.Exceptions;
using Cachara.Shared.Application.Results;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using FluentResults;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cachara.Content.API.Services.External;

public class PostService(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    ILogger<PostService> logger,
    IAggregateExceptionHandler exceptionHandler)
    : PostInternalService(postRepository), IPostService
{
    private readonly IPostRepository _postRepository = postRepository;

    public async Task<Result<Post>> GetById(string id)
    {
        try
        {
            var post = await _postRepository.FindByAsync(x => x.Id == id)
                       ?? throw new NotFoundException("Post not found");

            return Result.Ok(post.Adapt<Post>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving post with ID {PostId}", id);
            return exceptionHandler.Handle(ex);
        }
    }

    public async Task<Result<PagedResult<Post>>> Search(PostSearchCommand searchCommand)
    {
        try
        {
            var query = _postRepository.GetEntities();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((searchCommand.Page - 1) * searchCommand.PageSize)
                .Take(searchCommand.PageSize)
                .ProjectToType<Post>()
                .ToListAsync();

            var pagedResult = new PagedResult<Post>
            {
                Items = items,
                TotalCount = totalCount,
                Page = searchCommand.Page,
                PageSize = searchCommand.PageSize
            };

            return Result.Ok(pagedResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching posts with filter {@SearchCommand}", searchCommand);
            return exceptionHandler.Handle(ex);
        }
    }

    public async Task<Result> Delete(string id)
    {
        try
        {
            var post = await _postRepository.FindByAsync(x => x.Id == id)
                       ?? throw new NotFoundException("Post not found");

            await _postRepository.RemoveAsync(post);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting post with ID {PostId}", id);
            return exceptionHandler.Handle(ex);
        }
    }

    public async Task<Result<Post>> Upsert(PostUpsert upsert)
    {
        try
        {
            var entityPost = await _postRepository.FindByAsync(x => x.Id == upsert.Id)
                             ?? throw new NotFoundException("Post not found");

            entityPost = entityPost == null
                ? await InsertInternal(new Domain.Entities.Post(), user => UpdateFromInternal(user, upsert))
                : await UpdateInternal(entityPost, user => UpdateFromInternal(user, upsert));

            await unitOfWork.Commit();

            return Result.Ok(entityPost.Adapt<Post>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error upserting post {@Upsert}", upsert);
            return exceptionHandler.Handle(ex);
        }
    }
}


