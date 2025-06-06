using System.Security.Claims;
using Cachara.Users.API.Services.Abstractions;

namespace Cachara.Users.API.API.Authentication;

// TODO: Work On userAccountService
public class UserAccountService : IAccountService<UserAccount>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IJwtProvider _jwtProvider;
    private UserAccount _current;

    public UserAccountService(IHttpContextAccessor contextAccessor, IJwtProvider jwtProvider)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
    }

    public UserAccount Current => _current ??= GetUserAccount();

    private UserAccount GetUserAccount()
    {
        var token = _contextAccessor.HttpContext?.Request
            .Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("Token is missing or invalid.");
        }

        var account = _jwtProvider.GetAccount(token);
        if (account == null)
        {
            throw new UnauthorizedAccessException("Unable to retrieve account information.");
        }

        return account;
    }

    public UserAccount SignIn(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Authorization token is missing.");

        var claimsPrincipal = _jwtProvider.GetAccount(token);

        if (claimsPrincipal == null)
            throw new UnauthorizedAccessException("Invalid token.");

        return new UserAccount
        {
            Id = claimsPrincipal.Claims.FirstOrDefault(p  => p.Type == ClaimTypes.NameIdentifier)?.Value
                 ?? throw new ArgumentException("Unable to retrieve user id from token."),
            UserName = claimsPrincipal.Claims.FirstOrDefault(p  => p.Type == ClaimTypes.Name)?.Value ?? "Unknown",
            FullName = claimsPrincipal.Claims.FirstOrDefault(p  => p.Type == "full_name")?.Value ?? "Unknown",
            Handle = claimsPrincipal.Claims.FirstOrDefault(p  => p.Type == "handle")?.Value ?? "Unknown",
            Claims = claimsPrincipal.Claims
        };
    }

    // Helper method to get the token from Authorization header
    private string GetAuthorizationToken()
    {
        var token = _contextAccessor.HttpContext?.Request
            .Headers["Authorization"].ToString().Replace("Bearer ", "");

        return token;
    }

    // Get specific claim from the current user
    public string GetClaimValue(string claimType)
    {
        var token = GetAuthorizationToken();
        var claimsPrincipal = _jwtProvider.GetAccount(token);

        if (claimsPrincipal == null) throw new UnauthorizedAccessException("Invalid token.");

        return claimsPrincipal.Claims.FirstOrDefault(p => p.Type == claimType)?.Value;
    }
}

