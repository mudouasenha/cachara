using AutoFixture;
using Cachara.Users.API.Domain.Entities;

namespace Cachara.Tests.Builders.Entities;

public class UserRoleBuilder : BaseBuilder<UserRole, UserRoleBuilder>
{
    public override UserRoleBuilder BuildDefault()
    {
        Object = new UserRole()
        {
            Id = Fixture.Create<Guid>().ToString(),
            UserId = Fixture.Create<Guid>().ToString(),
            User = new UserBuilder().BuildDefault().Create(),
            RoleId = Fixture.Create<RoleType>(),
            Role = new RoleBuilder().BuildDefault().Create(),
            AssignedDate = Fixture.Create<DateTimeOffset>(),
            CreatedAt = Fixture.Create<DateTimeOffset>(),
            UpdatedAt = Fixture.Create<DateTimeOffset?>(),
            Deleted = false,
            Version = Fixture.Create<byte[]>()
        };

        return this;
    }

    public UserRoleBuilder WithId(string id)
    {
        Object.Id = id;
        return this;
    }

    public UserRoleBuilder WithUserId(string userId)
    {
        Object.UserId = userId;
        return this;
    }

    public UserRoleBuilder WithUser(User user)
    {
        Object.User = user;
        Object.UserId = user?.Id;
        return this;
    }

    public UserRoleBuilder WithRoleId(RoleType roleId)
    {
        Object.RoleId = roleId;
        return this;
    }

    public UserRoleBuilder WithRole(Role role)
    {
        Object.Role = role;
        Object.RoleId = role.Id;
        return this;
    }

    public UserRoleBuilder WithAssignedDate(DateTimeOffset assignedDate)
    {
        Object.AssignedDate = assignedDate;
        return this;
    }

    public UserRoleBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        Object.CreatedAt = createdAt;
        return this;
    }

    public UserRoleBuilder WithUpdatedAt(DateTimeOffset? updatedAt)
    {
        Object.UpdatedAt = updatedAt;
        return this;
    }

    public UserRoleBuilder AsDeleted()
    {
        Object.Deleted = true;
        return this;
    }

    public UserRoleBuilder AsNotDeleted()
    {
        Object.Deleted = false;
        return this;
    }

    public UserRoleBuilder WithVersion(byte[] version)
    {
        Object.Version = version;
        return this;
    }

    public UserRoleBuilder AsAdminAssignment()
    {
        Object.RoleId = RoleType.Admin;
        Object.AssignedDate = DateTimeOffset.UtcNow;
        return this;
    }

    public UserRoleBuilder AsUserAssignment()
    {
        Object.RoleId = RoleType.User;
        Object.AssignedDate = DateTimeOffset.UtcNow;
        return this;
    }

    public UserRoleBuilder AsRecentlyAssigned()
    {
        Object.AssignedDate = DateTimeOffset.UtcNow;
        Object.CreatedAt = DateTimeOffset.UtcNow;
        Object.UpdatedAt = null;
        return this;
    }

    public UserRoleBuilder ForUserAndRole(string userId, RoleType roleId)
    {
        Object.UserId = userId;
        Object.RoleId = roleId;
        return this;
    }
}