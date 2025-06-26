using System.Security.Claims;
using Cachara.Shared.Domain.Entities;
using Cachara.Shared.Infrastructure;
using Cachara.Users.API.Services.Models.Internal;

namespace Cachara.Users.API.Infrastructure.Security;

public interface IJwtProvider
{
    TokenResult Generate(Domain.Entities.User user);
    UserAccount GetAccount(string token);
    ClaimsPrincipal DecodeToken(string token);
}
