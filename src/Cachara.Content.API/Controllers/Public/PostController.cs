using Cachara.Content.API.Domain.Entities;
using Cachara.Content.API.Infrastructure;
using Cachara.Content.API.Services;
using Cachara.Domain.Commands;
using Cachara.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.Controllers.Public;

[ApiController]
[Route("api/public/post")]
[TagGroup("Post")]
public class PostController : ControllerBase
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
    public async Task<IEnumerable<Post>> Search(PostSearchCommand search)
    {
        return await _postService.Search(search);
    }

    [HttpGet("{id}")]
    public async Task<Post> GetById(string id)
    {
        return await _postService.GetById(id);
    }

    [HttpPost]
    public async Task<Post> Create(PostUpsert upsert)
    {
        _logger.LogInformation("Creating a Post named {Title}, Which AuthorId is {AuthorId}", upsert.Title,
            upsert.AuthorId);
        return await _postService.Upsert(upsert);
    }

    [HttpDelete("{id}")]
    public async Task Delete(string id)
    {
        await _postService.Delete(id);
    }

    [HttpPost("read/{id}")]
    public async Task<Post> ReadPost(string id)
    {
        return await _postService.GetById(id);
    }
}