using Cachara.Shared.Infrastructure.Hangfire;
using FluentResults;

namespace Cachara.Content.API.Services.Internal;

public class PostManagerService : IPostManagerService
{
    //private readonly IPostRepository _postRepository;
    private readonly IBackgroundServiceManager _backgroundServiceManager;

    public PostManagerService(IBackgroundServiceManager backgroundServiceManager)
    {
        //_logger = logger;
        _backgroundServiceManager = backgroundServiceManager;
        //_postRepository = postRepository;
    }

    public Task<Result> ExportPosts(string userId)
    {
        _backgroundServiceManager.Enqueue<IPostManagerService>(x => x.ExportPostsInternal(userId));
        return Task.FromResult(new Result());
    }

    public Task<Result> ExportPostsInternal(string userId)
    {
        Console.WriteLine($"Exporting Posts for userId {userId}");
        return Task.FromResult(new Result());
    }
}
