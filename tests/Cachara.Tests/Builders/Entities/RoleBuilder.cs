using AutoFixture;
using Cachara.Users.API.Domain.Entities;

namespace Cachara.Tests.Builders.Entities;

public class RoleBuilder : BaseBuilder<Role, RoleBuilder>
{
    public override RoleBuilder BuildDefault()
    {
        Object = new Role()
        {
            Id = Fixture.Create<RoleType>(),
            Description = Fixture.Create<string>(),
            UserRoles = Fixture.CreateMany<UserRole>().ToList(),
            CreatedAt = Fixture.Create<DateTimeOffset>(),
            UpdatedAt = Fixture.Create<DateTimeOffset?>(),
            Deleted = false,
            Version = Fixture.Create<byte[]>()
        };

        return this;
    }

    public RoleBuilder WithId(RoleType id)
    {
        Object.Id = id;
        return this;
    }

    public RoleBuilder WithDescription(string description)
    {
        Object.Description = description;
        return this;
    }

    public RoleBuilder WithUserRoles(params UserRole[] userRoles)
    {
        Object.UserRoles = userRoles.ToList();
        return this;
    }

    public RoleBuilder AddUserRole(UserRole userRole)
    {
        Object.UserRoles.Add(userRole);
        return this;
    }

    public RoleBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        Object.CreatedAt = createdAt;
        return this;
    }

    public RoleBuilder WithUpdatedAt(DateTimeOffset? updatedAt)
    {
        Object.UpdatedAt = updatedAt;
        return this;
    }

    public RoleBuilder AsDeleted()
    {
        Object.Deleted = true;
        return this;
    }

    public RoleBuilder AsNotDeleted()
    {
        Object.Deleted = false;
        return this;
    }

    public RoleBuilder WithVersion(byte[] version)
    {
        Object.Version = version;
        return this;
    }

    public RoleBuilder AsAdminRole()
    {
        Object.Id = RoleType.Admin;
        Object.Description = "Administrator role with full permissions";
        return this;
    }

    public RoleBuilder AsUserRole()
    {
        Object.Id = RoleType.User;
        Object.Description = "Standard user role with basic permissions";
        return this;
    }

    public RoleBuilder AsNewRole()
    {
        Object.CreatedAt = DateTimeOffset.UtcNow;
        Object.UpdatedAt = null;
        Object.Deleted = false;
        return this;
    }
}