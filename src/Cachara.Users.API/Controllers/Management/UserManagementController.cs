using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Externals;
using Microsoft.AspNetCore.Mvc;

namespace Cachara.Users.API.Controllers.Management;

[ApiController]
[Route("api/management/[controller]")]
[ApiExplorerSettings(GroupName = "management")]
[Tags("User")]
public class UserManagementController() : ControllerBase
{
}
