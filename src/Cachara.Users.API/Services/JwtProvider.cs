using Cachara.Users.API.API.Options;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Abstractions;

namespace Cachara.Users.API.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtProvider : IJwtProvider
{
    private JwtOptions _jwtOptions;
    private readonly SymmetricSecurityKey _key;

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
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("username", user.UserName),
            new Claim("handle", user.Handle),
            new Claim("profile_picture", user.ProfilePictureUrl ?? string.Empty)
        };

        foreach (var role in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
        }
        
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuers.First(),
            audience: _jwtOptions.Audiences.First(),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
