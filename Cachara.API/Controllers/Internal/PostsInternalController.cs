using Cachara.API.Controllers.Public;
using Cachara.Domain.Commands;
using Cachara.Domain.Entities;
using Cachara.Domain.Interfaces.Services;
using Cachara.Services.Internal;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.API.Controllers.Internal;

public class PostsInternalController
{
    private readonly ILogger<PostsController> _logger;
        private readonly PostManagerService _postManagerService;

        public PostsInternalController(PostManagerService postManagerService, ILogger<PostsController> logger)
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