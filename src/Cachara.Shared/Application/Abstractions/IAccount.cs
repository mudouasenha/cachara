
using Cachara.Shared.Infrastructure.Security;

namespace Cachara.Shared.Application.Abstractions;

public interface IAccount
{
    string Id { get; }

    string FullName { get; }

    IEnumerable<Claim> Claims { get; }
}
