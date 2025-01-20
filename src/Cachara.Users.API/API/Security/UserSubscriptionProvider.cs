using Microsoft.Extensions.Caching.Hybrid;

namespace Cachara.Users.API.API.Security;

// ApiKeyProvider
// Purpose: A provider typically serves as a source of API keys. It is responsible for generating, retrieving, and/or validating API keys.
// Responsibilities:
// Generate API Keys: Create new API keys that can be issued to clients or users.
//     Retrieve Keys: Fetch stored API keys from a database, configuration file, or other storage mechanisms.
//     Key Metadata: Provide information about the API key, such as expiration dates, permissions, or associated user/client information.
//     Focus: The focus is on managing the API keys themselves, not necessarily how they are used for authentication/authorization.
//     Example Use Cases:
// Generating an API key when a new client account is created.
// Retrieving an API key from a database for validation.
public class UserSubscriptionProvider // IApiKeyProvider
{
    public HybridCache hybridCache;

    public Subscription GetSubscription(string? userId)
    {
        // TODO: Use a database for this.
        if (string.IsNullOrEmpty(userId))
        {
            return Subscription.Standard;
        }

        return Subscription.Management;
    }

    // public Task<IApiKey> ProvideAsync(string key)
    // {
    //         
    //     if (string.IsNullOrEmpty(key))
    //     {
    //         return Task.FromResult<IApiKey>(Subscription.Standard);
    //     }
    //
    //     return Subscription.Management;
    // }
}
