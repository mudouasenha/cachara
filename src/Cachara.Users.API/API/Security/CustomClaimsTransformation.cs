using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Cachara.Users.API.API.Security;

/// <summary>
///     Custom Claiim Transformation if claim not defined
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <see cref="https://www.youtube.com/watch?v=cgjifZF8ZME" />
public class CustomClaimsTransformation(IServiceScopeFactory serviceScopeFactory) : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(p => p.Type == CustomClaims.Subscription))
        {
            return Task.FromResult(principal);
        }

        using var scope = serviceScopeFactory.CreateScope();

        var subscriptionProvider =
            scope.ServiceProvider.GetRequiredService<UserSubscriptionProvider>(); // TODO: Introduce Caching

        var subscription = subscriptionProvider.GetSubscription(principal.Identity?.Name);

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(CustomClaims.Subscription, subscription.ToString()));

        principal.AddIdentity(identity);

        return Task.FromResult(principal);
    }
}
