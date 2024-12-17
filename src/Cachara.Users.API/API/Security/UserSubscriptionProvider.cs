using Microsoft.IdentityModel.Tokens;

namespace Cachara.Users.API.API.Security;

public class UserSubscriptionProvider
{
    public Subscription GetSubscription(string? userId)
    { // TODO: Use a database for this.
        if (string.IsNullOrEmpty(userId))
        {
            return Subscription.Standard;
        }

        return Subscription.Management;
    }
}