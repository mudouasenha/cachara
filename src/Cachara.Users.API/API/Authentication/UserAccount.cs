using System.Security.Claims;

namespace Cachara.Users.API.API.Authentication;

public class UserAccount : IAccount
{
    public string Id { get; }
    public string FullName { get; }
    public string UserName { get; set; }
    public string Handle { get; set; }
    public IEnumerable<Claim> Claims { get; }
}