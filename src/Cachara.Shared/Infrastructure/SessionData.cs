
using Cachara.Users.API.API.Authentication;

namespace Cachara.Shared.Infrastructure;

public class SessionData
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public IEnumerable<IClaim> Claims { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
