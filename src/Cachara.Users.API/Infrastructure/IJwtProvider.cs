using System.Security.Claims;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.Services.Models;
using Cachara.Users.API.Services.Models.Internal;

namespace Cachara.Users.API.Infrastructure;

public interface IJwtProvider
{
    TokenResult Generate(Domain.Entities.User user);
    UserAccount GetAccount(string token);
    ClaimsPrincipal DecodeToken(string token);
}
