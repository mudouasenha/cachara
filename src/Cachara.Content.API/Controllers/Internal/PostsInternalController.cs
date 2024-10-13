using Cachara.Content.API.Controllers.Public;
using Cachara.Domain.Commands;
using Cachara.Services.Internal;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.Controllers.Internal;

public class PostsInternalController
{
    private readonly ILogger<PostController> _logger;
    private readonly PostManagerService _postManagerService;

    public PostsInternalController(PostManagerService postManagerService, ILogger<PostController> logger)
    {
        _postManagerService = postManagerService;
        _logger = logger;
    }

    [HttpPost("export")]
    public async Task<Result> ExportPosts(PostSearchCommand search)
    {
        return await _postManagerService.ExportPosts(search.Author);
    }
}