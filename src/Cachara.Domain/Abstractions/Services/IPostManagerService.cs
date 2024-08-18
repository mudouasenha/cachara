using FluentResults;

namespace Cachara.Domain.Interfaces.Services;

public interface IPostManagerService
{
    Task<Result> ExportPosts(string userId);
    Task<Result> ExportPostsInternal(string userId);
}