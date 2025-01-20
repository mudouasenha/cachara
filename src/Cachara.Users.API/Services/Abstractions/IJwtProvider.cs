using Cachara.Users.API.Domain.Entities;

namespace Cachara.Users.API.Services.Abstractions;

public interface IJwtProvider
{
    string Generate(User user);
    string GenerateRefreshToken();
}
