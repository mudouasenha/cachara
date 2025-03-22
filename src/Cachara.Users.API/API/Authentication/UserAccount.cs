using System.Security.Claims;
using Cachara.Shared.Infrastructure;

namespace Cachara.Users.API.API.Authentication;

public class UserAccount : IAccount
{
    public string UserName { get; set; }
    public string Handle { get; set; }
    public string Id { get; set; }
    public string FullName { get; set; }
    public IEnumerable<Claim> Claims { get; set; } // TODO: handle private and public accessors
}
