using FluentResults;

namespace Cachara.Content.API.Services;

public interface IPostManagerService
{
    Task<Result> ExportPosts(string userId);
    Task<Result> ExportPostsInternal(string userId);
}
