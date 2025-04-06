using System.Security.Claims;

namespace Cachara.Shared.Infrastructure;

public class SessionData
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
