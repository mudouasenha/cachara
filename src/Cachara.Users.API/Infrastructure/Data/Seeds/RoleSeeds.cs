using Cachara.Users.API.Domain.Entities;

namespace Cachara.Users.API.Infrastructure.Data.Seeds;

public static class RoleSeeds
{
    public static List<Role> GetUserRoles()
    {
        return new List<Role>
        {
            new() { Id = RoleType.User, Description = "User" },
            new() { Id = RoleType.Admin, Description = "Administrator" }
        };
    }
}
