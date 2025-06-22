using Cachara.Content.API.Domain.Commands;
using Cachara.Content.API.Services.Models;
using Cachara.Shared.Application.Results;
using FluentResults;

namespace Cachara.Content.API.Services;

public interface IPostService
{
    public Task<Result<Post>> GetById(string id);
    public Task<Result<PagedResult<Post>>> Search(PostSearchCommand searchCommand);
    public Task<Result<Post>> Upsert(PostUpsert upsert);
    public Task<Result> Delete(string id);
}
