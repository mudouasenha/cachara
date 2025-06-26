
using Cachara.Shared.Infrastructure.Security;

namespace Cachara.Shared.Infrastructure.Session;

public class SessionData
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
