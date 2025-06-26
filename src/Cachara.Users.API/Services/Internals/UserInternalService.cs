using Cachara.Shared.Domain;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Services.Models;

namespace Cachara.Users.API.Services.Internals;

public abstract class UserInternalService(
    IUserRepository userRepository,
    IGeneralDataProtectionService generalDataProtectionService)
{
    // TODO: Use Password Hashing Instead of Encryption
    /// <summary>
    /// For passwords, encryption is not recommended.
    /// ✅ Use a password hash algorithm (bcrypt, PBKDF2, Argon2).
    /// ✅ Store the salt with the hash.
    /// ✅ On password change, rehash and compare.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newPassword"></param>
    /// <see>
    ///     <cref>https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html</cref>
    ///     <cref>https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-8.0</cref>
    /// </see>
    protected async Task<Domain.Entities.User> UpdatePassword(
        Domain.Entities.User user,
        string newPassword
    )
    {
        var newPasswordEncrypted = generalDataProtectionService.EncryptString(newPassword);
        return await UpdateInternal(user, userUpdate => userUpdate.Password = newPasswordEncrypted);
    }

    internal void UpdateFromInternal(Domain.Entities.User user, UserUpsert upsert)
    {
        var encryptedPassword = generalDataProtectionService.EncryptString(upsert.Password);

        user.Email = upsert.Email.ToLowerInvariant();
        user.FullName = upsert.FullName;
        user.DateOfBirth = upsert.DateOfBirth;
        user.UserName = upsert.UserName.ToLowerInvariant();
        user.Password = encryptedPassword;
        user.Subscription = upsert.Subscription;

        user.ValidateAndThrow();
    }

    internal async Task<Domain.Entities.User> FindById(string id)
    {
        var normalizedId = id.ToLowerInvariant();
        var user = await userRepository.FindByAsync(p => p.Id == normalizedId);

        return user;
    }

    internal async Task<Domain.Entities.User> FindByUserName(string userName)
    {
        var normalizedUserName = userName.ToLowerInvariant();
        var user = await userRepository.FindByAsync(p => p.UserName == normalizedUserName);

        return user;
    }

    internal async Task<Domain.Entities.User> FindByEmail(string id)
    {
        var normalizedEmail = id.ToLowerInvariant();
        var user = await userRepository.FindByAsync(p => p.Email == normalizedEmail);

        return user;
    }

    internal async Task<Domain.Entities.User> UpdateInternal(
        Domain.Entities.User user,
        Action<Domain.Entities.User> entityUpdate = null
    )
    {
        user.UpdatedAt = DateTimeOffset.UtcNow;

        entityUpdate?.Invoke(user);

        await userRepository.EditAsync(user);
        return user;
    }

    internal async Task<Domain.Entities.User> InsertInternal(
        Domain.Entities.User user,
        Action<Domain.Entities.User> entityUpdate = null
    )
    {
        user.GenerateId();
        user.UpdateCreatedAt();

        entityUpdate?.Invoke(user);

        await userRepository.AddAsync(user);
        return user;
    }
}
