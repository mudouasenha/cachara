using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Models.Internal;

namespace Cachara.Users.API.Services.Abstractions;

public interface IJwtProvider
{
    TokenResult Generate(User user);
}
