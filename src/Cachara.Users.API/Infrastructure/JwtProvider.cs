using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.API.Options;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Models.Internal;
using Microsoft.IdentityModel.Tokens;
using Claim = System.Security.Claims.Claim;

namespace Cachara.Users.API.Infrastructure;

public class JwtProvider : IJwtProvider
{
    private readonly SymmetricSecurityKey _key;
    private readonly JwtOptions _jwtOptions;

    public JwtProvider(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
    }

    public TokenResult Generate(User user)
    {
        var now = DateTimeOffset.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new("userId", user.Id),
            new("username", user.UserName),
            new("fullName", user.FullName),
            new("dateOfBirth", user.DateOfBirth.ToString(CultureInfo.InvariantCulture)),
        };

        foreach (var role in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Id.ToString()));
        }

        var jwtToken = new JwtSecurityToken(
            _jwtOptions.Issuers.First(),
            _jwtOptions.Audiences.First(),
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
        );

        var refreshToken = GenerateRefreshToken();

        return new TokenResult()
        {
            ExpiresAt = jwtToken.ValidTo,
            RefreshToken = refreshToken,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
        };
    }

    public UserAccount GetAccount(string token)
    {
        var claims = DecodeToken(token);
        return new UserAccount()
        {
            UserName = GetClaimValue(token, "username") ?? throw new SecurityTokenException("Missing 'username' claim in token"),
            FullName = GetClaimValue(token, "fullName") ?? throw new SecurityTokenException("Missing 'fullName' claim in token"),
            Claims = claims.Claims.Select(API.Authentication.Claim.FromClaim),
            Id = GetClaimValue(token, "userId") ?? throw new SecurityTokenException("Missing 'userId' claim in token")
            //Handle = GetClaimValue(token, "handle") ?? throw new SecurityTokenException("Missing 'handle' claim in token")
        };
    }


    public ClaimsPrincipal DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            return null;

        var jwtToken = handler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(jwtToken.Claims);
        return new ClaimsPrincipal(identity);
    }

    private string GetClaimValue(string token, string claimType)
    {
        var claimsPrincipal = DecodeToken(token);
        return claimsPrincipal?.FindFirst(claimType)?.Value;
    }

    public string GetUserId(string token)
        => GetClaimValue(token, ClaimTypes.NameIdentifier);

    public string GetTenantId(string token)
        => GetClaimValue(token, "tenant_id");

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }
}
