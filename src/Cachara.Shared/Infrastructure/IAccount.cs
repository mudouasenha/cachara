using System.Security.Claims;

namespace Cachara.Shared.Infrastructure;

public interface IAccount
{
    string Id { get; }

    string FullName { get; }

    IEnumerable<IClaim> Claims { get; }
}
