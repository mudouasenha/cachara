using Cachara.Users.API.Domain.Entities;
using Mapster;

namespace Cachara.Users.API.Services.Mappings;

public static class UsersMappings
{
    public static void Configure()
    {
        TypeAdapterConfig<User, Services.Models.User>.NewConfig();
    }
}
