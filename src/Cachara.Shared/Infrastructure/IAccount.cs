
using Cachara.Users.API.API.Authentication;

namespace Cachara.Shared.Infrastructure;

public interface IAccount
{
    string Id { get; }

    string FullName { get; }

    IEnumerable<Claim> Claims { get; }
}
