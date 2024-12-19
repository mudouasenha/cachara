using System.Security.Claims;

namespace Cachara.Users.API.API.Authentication;

public interface IAccountService<TAccount> where TAccount : class, IAccount 
{
    TAccount Current { get; }

    Task<bool> SignIn(string provider, string key);
}

public interface IAccount
{
    string Id { get; }

    string FullName { get; }

    IEnumerable<Claim> Claims { get; }
}