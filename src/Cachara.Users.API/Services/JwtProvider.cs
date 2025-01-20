using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cachara.Users.API.API.Options;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Abstractions;
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

    public string Generate(User user)
    {
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new("Id", user.Id),
            new("username", user.UserName),
            new("fullName", user.FullName),
            new("dateOfBirth", user.DateOfBirth.ToString(CultureInfo.InvariantCulture)),
            new("profile_picture", user.ProfilePictureUrl ?? string.Empty)
        };

        foreach (var role in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
        }

        var token = new JwtSecurityToken(
            _jwtOptions.Issuers.First(),
            _jwtOptions.Audiences.First(),
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }
}
