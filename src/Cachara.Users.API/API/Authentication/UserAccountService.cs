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

    public UserAccount Current { get; }

    public Task<bool> SignIn(string provider, string key)
    {
        throw new NotImplementedException();
    }

    public string GetUserId()
    {
        return _contextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
    }
}
