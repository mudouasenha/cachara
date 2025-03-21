using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    public IActionResult HandleFailure()
    {
        throw new NotImplementedException("Please implement HandleFailure Controller Method.");
    }
}
