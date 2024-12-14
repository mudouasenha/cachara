using Cachara.Content.API.Services;
using Cachara.Shared.Infrastructure.Hangfire;
using FluentResults;

namespace Cachara.Services.Internal;

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

    public async Task<Result> ExportPosts(string userId)
    {
        _backgroundServiceManager.Enqueue<IPostManagerService>(x => x.ExportPostsInternal(userId));
        return new Result();
    }
    
    public async Task<Result> ExportPostsInternal(string userId)
    {
        Console.WriteLine($"Exporting Posts for userId {userId}");
        return new Result();
    }
}