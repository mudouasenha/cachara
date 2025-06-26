using Cachara.Content.API.API.Controllers.Public;
using Cachara.Content.API.Domain.Commands;
using Cachara.Content.API.Services.External;
using Cachara.Content.API.Services.Internal;
using Cachara.Content.API.Services.Models;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.API.Controllers.Internal;

[ApiExplorerSettings(GroupName = "internal")]
[Route("internal/devtest")]
[Tags("Posts")]
public class PostsInternalController(InternalPostService postService, ILogger<PostController> logger)
{
    [HttpGet()]
    public async Task<List<Post>> GetPosts()
    {
        var result = await postService.Get();

        return result;
    }


    [HttpGet()]
    public async Task<List<Post>> UpsertPost()
    {
        var result = await postService.Get();

        return result;
    }
}
