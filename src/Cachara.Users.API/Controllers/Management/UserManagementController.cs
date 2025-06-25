using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Externals;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Management;

[ApiController]
[Route("management/[controller]")]
[ApiExplorerSettings(GroupName = "user")]
[Tags("User")]
public class UserManagementController() : ControllerBase
{
}
