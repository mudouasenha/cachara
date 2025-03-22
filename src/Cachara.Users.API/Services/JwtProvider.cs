using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.API.Options;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Models.Internal;
using Microsoft.IdentityModel.Tokens;

namespace Cachara.Users.API.Services;

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
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToString("o"), ClaimValueTypes.String),
            new("Id", user.Id),
            new("username", user.UserName),
            new("fullName", user.FullName),
            new("dateOfBirth", user.DateOfBirth.ToString(CultureInfo.InvariantCulture)),
        };

        foreach (var role in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name.ToString()));
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

    public UserAccount Decode(string token)
    {
        throw new NotImplementedException();
    }

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
