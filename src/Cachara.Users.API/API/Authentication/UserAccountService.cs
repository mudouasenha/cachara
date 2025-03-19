using System.Security.Claims;

namespace Cachara.Users.API.API.Authentication;

// ApiKeyService
// Purpose: A service typically provides business logic and operations surrounding the use of API keys for authentication and authorization.
// Responsibilities:
// Validate API Keys: Ensure an API key provided by a client is valid, not expired, and has sufficient permissions.
// Authorize Requests: Verify that the API key has the necessary permissions to perform specific actions.
// Revoke Keys: Handle invalidation or deactivation of API keys.
// Focus: The focus is on applying the API keys to authenticate and authorize users or systems.
// Example Use Cases:
// Validating an API key included in a request.
// Checking whether an API key has the necessary permissions to access a resource.
public class UserAccountService : IAccountService<UserAccount>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserAccountService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    // Current user account
    public UserAccount Current { get; protected set; }

    public void SignIn()
    {
        var token = GetAuthorizationToken();

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Authorization token is missing.");

        var claimsPrincipal = JwtTokenHelper.DecodeToken(token);

        if (claimsPrincipal == null)
            throw new UnauthorizedAccessException("Invalid token.");

        Current = new UserAccount
        {
            Id = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown",
            UserName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",
            FullName = claimsPrincipal.FindFirst("full_name")?.Value ?? "Unknown",
            Handle = claimsPrincipal.FindFirst("handle")?.Value ?? "Unknown",
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
        var claimsPrincipal = JwtTokenHelper.DecodeToken(token);

        if (claimsPrincipal == null) throw new UnauthorizedAccessException("Invalid token.");

        return claimsPrincipal.FindFirst(claimType)?.Value;
    }
}

