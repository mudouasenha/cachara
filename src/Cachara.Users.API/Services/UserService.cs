using Cachara.Shared.Domain;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Cachara.Users.API.Services;

public class UserService : IUserService
{
    private readonly IGeneralDataProtectionService _generalDataProtectionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IGeneralDataProtectionService generalDataProtectionService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _generalDataProtectionService = generalDataProtectionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> GetByUserName(string userName)
    {
        return await _userRepository.FindByAsync(p => p.UserName == userName);
    }

    public Task<IEnumerable<User>> Search(UserSearchCommand searchCommmand)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserById(string id)
    {
        var user = await _userRepository.FindByAsync(p => p.Id == id)
                   ?? throw new Exception("User Not Found!");

        user.Password = _generalDataProtectionService.DecryptString(user.Password);

        return user;
    }

    public async Task<User> GetWithPostsById(string id)
    {
        return await _userRepository
            .GetEntities()
            .FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("User Not Found!");
    }

    public async Task<User> Upsert(UserUpsert upsert)
    {
        var expression = (User x) => x.Id == upsert.Id;
        var entityUser = await _userRepository.FindByAsync(x => x.Id == upsert.Id);
        if (entityUser is null && upsert.Id is not null)
        {
            throw new Exception("User not found");
        }

        entityUser = entityUser == null
            ? await InsertInternal(new User(), user => UpdateFromInternal(user, upsert))
            : await UpdateInternal(entityUser, user => UpdateFromInternal(user, upsert));

        await _unitOfWork.Commit();
        return entityUser;
    }


    public async Task<User> CreateUser(UserUpsert upsert)
    {
        var user = new User
        {
            Email = upsert.Email,
            UserName = upsert.UserName,
            FullName = upsert.FullName,
            Subscription = upsert.Subscription,
            DateOfBirth = upsert.DateOfBirth,
            Settings = new UserSettings()
        };

        user.AssignRole(RoleType.User);
        user.Password = _generalDataProtectionService.EncryptString(upsert.Password);

        user.GenerateId();
        user.UpdateCreatedAt();
        user.Settings.UpdateCreatedAt();
        user.Settings.GenerateId();

        await _userRepository.AddAsync(user);

        await _unitOfWork.Commit();

        return await GetUserById(user.Id);
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _userRepository.FindByAsync(p => p.Email == email);
    }

    private void UpdateFromInternal(User user, UserUpsert upsert)
    {
        var encryptedPassword = _generalDataProtectionService.EncryptString(upsert.Password);

        user.Email = upsert.Email;
        user.FullName = upsert.FullName;
        user.DateOfBirth = upsert.DateOfBirth;
        user.UserName = upsert.UserName;
        user.Password = encryptedPassword;
        user.Subscription = upsert.Subscription;

        user.ValidateAndThrow();
    }

    internal string DecryptPassword(User user)
    {
        var decryptedPassword = _generalDataProtectionService.DecryptString(user.Password);

        return decryptedPassword;
    }

    internal bool VerifyPassword(User user, string password)
    {
        var encryptedPassword = _generalDataProtectionService.EncryptString(password);

        return user.Password == encryptedPassword;
    }

    internal async Task<User> InsertInternal(
        User user,
        Action<User> entityUpdate = null
    )
    {
        user.GenerateId();
        user.UpdateCreatedAt();

        entityUpdate?.Invoke(user);

        await _userRepository.AddAsync(user);
        return user;
    }

    internal async Task<User> UpdateInternal(
        User user,
        Action<User> entityUpdate = null
    )
    {
        user.UpdatedAt = DateTimeOffset.UtcNow;

        entityUpdate?.Invoke(user);

        await _userRepository.EditAsync(user);
        return user;
    }
}
