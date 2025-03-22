using Cachara.Users.API.Domain.Entities;

namespace Cachara.Users.API.Infrastructure.Data.Seeds;

public static class RoleSeeds
{
    public static List<Role> GetUserRoles()
    {
        return new List<Role>
        {
            new() { Id = "0195bc08-3824-7836-8ae7-b9342b9f8444", Name = RoleType.User, Description = "User" },
            new() { Id = "0195bc08-632d-7af8-88f5-568b043e5aeb", Name = RoleType.Admin, Description = "Administrator" }
        };
    }
}
