using AutoFixture;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.Domain.Entities;

namespace Cachara.Tests.Builders.Entities;

public class UserBuilder : BaseBuilder<User, UserBuilder>
{
    public override UserBuilder BuildDefault()
    {
        Object = new User()
        {
            Id = Fixture.Create<Guid>().ToString(),
            FullName = Fixture.Create<string>(),
            UserName = Fixture.Create<string>(),
            Email = Fixture.Create<string>(),
            Password = Fixture.Create<string>(),
            DateOfBirth = DateOnly.FromDateTime(Fixture.Create<DateTime>()),
            Subscription = Fixture.Create<Subscription>(),
            Settings = Fixture.Create<UserSettings>(),
            UserRoles = Fixture.CreateMany<UserRole>().ToList(),
            CreatedAt = Fixture.Create<DateTimeOffset>(),
            UpdatedAt = Fixture.Create<DateTimeOffset?>()
        };

        return this;
    }

    public UserBuilder WithId(string id)
    {
        Object.Id = id;
        return this;
    }

    public UserBuilder WithFullName(string fullName)
    {
        Object.FullName = fullName;
        return this;
    }

    public UserBuilder WithUserName(string userName)
    {
        Object.UserName = userName;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        Object.Email = email;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        Object.Password = password;
        return this;
    }

    public UserBuilder WithDateOfBirth(DateOnly dateOfBirth)
    {
        Object.DateOfBirth = dateOfBirth;
        return this;
    }

    public UserBuilder WithAge(int age)
    {
        Object.DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-age));
        return this;
    }

    public UserBuilder WithSubscription(Subscription subscription)
    {
        Object.Subscription = subscription;
        return this;
    }

    public UserBuilder WithSettings(UserSettings settings)
    {
        Object.Settings = settings;
        return this;
    }

    public UserBuilder WithRoles(params UserRole[] roles)
    {
        Object.UserRoles = roles.ToList();
        return this;
    }

    public UserBuilder AddRole(UserRole role)
    {
        Object.UserRoles.Add(role);
        return this;
    }

    public UserBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        Object.CreatedAt = createdAt;
        return this;
    }

    public UserBuilder WithUpdatedAt(DateTimeOffset? updatedAt)
    {
        Object.UpdatedAt = updatedAt;
        return this;
    }

    public UserBuilder AsNewUser()
    {
        Object.CreatedAt = DateTimeOffset.UtcNow;
        Object.UpdatedAt = null;
        return this;
    }

    public UserBuilder AsRecentlyUpdated()
    {
        Object.UpdatedAt = DateTimeOffset.UtcNow;
        return this;
    }
}