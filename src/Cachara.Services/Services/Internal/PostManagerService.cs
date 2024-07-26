using Cachara.Data.Interfaces;
using FluentResults;

namespace Cachara.Services.Internal;

public class PostManagerService
{
    private readonly IPostRepository _postRepository;

    public PostManagerService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Result> ExportPosts(string userId)
    {
        return new Result();
    }
}