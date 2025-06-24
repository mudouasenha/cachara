using AutoFixture;
using Cachara.Users.API.Domain.Entities;

namespace Cachara.Tests.Builders.Entities;

public class UserSettingsBuilder : BaseBuilder<UserSettings, UserSettingsBuilder>
{
    public override UserSettingsBuilder BuildDefault()
    {
        Object = new UserSettings()
        {
            Id = Fixture.Create<Guid>().ToString(),
            UserId = Fixture.Create<Guid>().ToString(),
            User = Fixture.Create<User>(),
            IsPrivate = Fixture.Create<bool>(),
            ReceiveNotifications = Fixture.Create<bool>(),
            ShowEmail = Fixture.Create<bool>(),
            CreatedAt = Fixture.Create<DateTimeOffset>(),
            UpdatedAt = Fixture.Create<DateTimeOffset?>(),
            Deleted = false,
            Version = Fixture.Create<byte[]>()
        };

        return this;
    }

    public UserSettingsBuilder WithId(string id)
    {
        Object.Id = id;
        return this;
    }

    public UserSettingsBuilder WithUserId(string userId)
    {
        Object.UserId = userId;
        return this;
    }

    public UserSettingsBuilder WithUser(User user)
    {
        Object.User = user;
        Object.UserId = user?.Id;
        return this;
    }

    public UserSettingsBuilder WithIsPrivate(bool isPrivate)
    {
        Object.IsPrivate = isPrivate;
        return this;
    }

    public UserSettingsBuilder WithReceiveNotifications(bool receiveNotifications)
    {
        Object.ReceiveNotifications = receiveNotifications;
        return this;
    }

    public UserSettingsBuilder WithShowEmail(bool showEmail)
    {
        Object.ShowEmail = showEmail;
        return this;
    }

    public UserSettingsBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        Object.CreatedAt = createdAt;
        return this;
    }

    public UserSettingsBuilder WithUpdatedAt(DateTimeOffset? updatedAt)
    {
        Object.UpdatedAt = updatedAt;
        return this;
    }

    public UserSettingsBuilder AsDeleted()
    {
        Object.Deleted = true;
        return this;
    }

    public UserSettingsBuilder AsNotDeleted()
    {
        Object.Deleted = false;
        return this;
    }

    public UserSettingsBuilder WithVersion(byte[] version)
    {
        Object.Version = version;
        return this;
    }

    public UserSettingsBuilder AsPrivateProfile()
    {
        Object.IsPrivate = true;
        Object.ShowEmail = false;
        return this;
    }

    public UserSettingsBuilder AsPublicProfile()
    {
        Object.IsPrivate = false;
        Object.ShowEmail = true;
        return this;
    }

    public UserSettingsBuilder WithNotificationsEnabled()
    {
        Object.ReceiveNotifications = true;
        return this;
    }

    public UserSettingsBuilder WithNotificationsDisabled()
    {
        Object.ReceiveNotifications = false;
        return this;
    }

    public UserSettingsBuilder AsDefaultSettings()
    {
        Object.IsPrivate = false;
        Object.ReceiveNotifications = true;
        Object.ShowEmail = false;
        Object.Deleted = false;
        return this;
    }

    public UserSettingsBuilder AsNewSettings()
    {
        Object.CreatedAt = DateTimeOffset.UtcNow;
        Object.UpdatedAt = null;
        Object.Deleted = false;
        return this;
    }

    public UserSettingsBuilder ForUser(string userId)
    {
        Object.UserId = userId;
        return this;
    }
}