using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Content.API.Controllers;

    [ApiController]
    public abstract class ResultControllerBase : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.Errors.Any(e => e.Message.Contains("NotFound", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result.Errors);

            return BadRequest(result.Errors);
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok();

            if (result.Errors.Any(e => e.Message.Contains("NotFound", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result.Errors);

            return BadRequest(result.Errors);
        }
    }

