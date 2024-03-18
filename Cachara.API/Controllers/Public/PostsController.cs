using Cachara.API.Infrastructure;
using Cachara.Domain.Commands;
using Cachara.Domain.Entities;
using Cachara.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.API.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [TagGroup("Post")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostService _postService;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpPost("search")]
        public async Task<IEnumerable<Post>> Search(PostSearchCommand search)
        {
            return await _postService.Search(search);
        }

        [HttpGet("{id}")]
        public async Task<Post> GetById(string id)
        {
            return await _postService.GetPostById(id);
        }

        [HttpPost()]
        public async Task<Post> Create(PostCreateCommand create)
        {
            return await _postService.CreatePost(create);
        }

        [HttpPut()]
        public async Task<Post> Update(PostUpdateCommand update)
        {
            return await _postService.UpdatePost(update);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _postService.DeletePost(id);
        }

        [HttpPost("read/{id}")]
        public async Task<Post> ReadPost(string id)
        {
            return await _postService.GetPostById(id);
        }
    }
}