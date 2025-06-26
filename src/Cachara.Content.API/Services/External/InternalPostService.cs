using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Shared.Application.Errors;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Hangfire;
using FluentResults;

namespace Cachara.Content.API.Services.External;

public class InternalPostService(
    IBackgroundServiceManager backgroundServiceManager,
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    ILogger<PostService> logger,
    IAggregateExceptionHandler exceptionHandler)
    : PostService(postRepository, unitOfWork, logger, exceptionHandler)
{
    public Task<Result> ExportPosts(string userId)
    {
        backgroundServiceManager.Enqueue<IPostManagerService>(x => x.ExportPostsInternal(userId));
        return Task.FromResult(new Result());
    }

    public Task<Result> ExportPostsInternal(string userId)
    {
        Console.WriteLine($"Exporting Posts for userId {userId}");
        return Task.FromResult(new Result());
    }
}
