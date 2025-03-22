using System.Security.Claims;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Models.Internal;

namespace Cachara.Users.API.Services.Abstractions;

public interface IJwtProvider
{
    TokenResult Generate(User user);
    UserAccount Decode(string token);
    ClaimsPrincipal? DecodeToken(string token);
}
