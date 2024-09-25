using Cachara.API.Infrastructure;
using Cachara.Domain.Commands;
using Cachara.Domain.Entities;
using Cachara.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.API.Controllers.Public;

[ApiController]
[Route("api/public/user")]
[TagGroup("User")]
public class UserController : ControllerBase
{
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        [HttpPost("register")]
        public async Task<User> Register(UserUpsert upsert)
        {
            return await _userService.Upsert(upsert);
        }
        
        [HttpGet("{userName}")]
        public async Task<User> GetByUserName(string userName)
        {
            return await _userService.GetByUserName(userName);
        }

}