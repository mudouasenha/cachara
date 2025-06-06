using Cachara.Shared.Infrastructure;

namespace Cachara.Users.API.API.Authentication;

public interface IAccountService<out TAccount> where TAccount : class, IAccount
{
    TAccount Current { get; }
}


