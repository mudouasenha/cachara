using Cachara.Content.API.Domain.Commands;
using Cachara.Content.API.Services;
using Cachara.Content.API.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.API.Controllers.Public;

[ApiController]
[Route("public/[controller]")]
[Tags("Post")]
public class PostController : ResultControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostService _postService;

    public PostController(IPostService postService, ILogger<PostController> logger)
    {
        _postService = postService;
        _logger = logger;
    }


    [HttpPost("search")]
    [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 20, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Search(PostSearchCommand search)
    {
        var result = await _postService.Search(search);

        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _postService.GetById(id);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PostUpsert upsert)
    {
        var result = await _postService.Upsert(upsert);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _postService.Delete(id);

        return HandleResult(result);
    }

    [HttpPost("read/{id}")]
    public async Task<IActionResult> ReadPost(string id)
    {
        var result = await _postService.GetById(id);

        return HandleResult(result);
    }
}
