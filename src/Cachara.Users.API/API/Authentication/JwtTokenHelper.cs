using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cachara.Users.API.API.Authentication;

public static class JwtTokenHelper
{
    public static ClaimsPrincipal? DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            return null;

        var jwtToken = handler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(jwtToken.Claims);
        return new ClaimsPrincipal(identity);
    }

    private static string? GetClaimValue(string token, string claimType)
    {
        var claimsPrincipal = DecodeToken(token);
        return claimsPrincipal?.FindFirst(claimType)?.Value;
    }

    public static string? GetUserId(string token)
        => GetClaimValue(token, ClaimTypes.NameIdentifier);

    public static string? GetTenantId(string token)
        => GetClaimValue(token, "tenant_id");
}
