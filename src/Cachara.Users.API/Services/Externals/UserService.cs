using Cachara.Shared.Application.Errors;
using Cachara.Shared.Domain;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Internals;
using Cachara.Users.API.Services.Models;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using User = Cachara.Users.API.Services.Models.User;

namespace Cachara.Users.API.Services.Externals;

public class UserService : UserInternalService, IUserService
{
    private readonly IMapper _mapper;
    private readonly IGeneralDataProtectionService _generalDataProtectionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserService
    (
        IMapper mapper,
        IUserRepository userRepository,
        IGeneralDataProtectionService generalDataProtectionService,
        IUnitOfWork unitOfWork)
        : base(userRepository, generalDataProtectionService)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _generalDataProtectionService = generalDataProtectionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> GetByUserName(string userName)
    {
        var normalizedUserName = userName.ToLowerInvariant();
        var entity = await _userRepository.FindByAsync(p => p.UserName == normalizedUserName);
        return _mapper.Map<User>(entity);
    }

    public Task<IEnumerable<User>> Search(UserSearchCommand searchCommmand)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserById(string id)
    {
        var normalizedId = id.ToLowerInvariant();
        var user = await _userRepository.FindByAsync(p => p.Id == normalizedId)
                   ?? throw new NotFoundException("User Not Found!");

        user.Password = _generalDataProtectionService.DecryptString(user.Password);

        return _mapper.Map<User>(user);
    }

    public async Task<User> GetWithPostsById(string id)
    {
        var user = await _userRepository
            .GetEntities()
            .FirstOrDefaultAsync(p => p.Id == id) ?? throw new NotFoundException("User Not Found!");

        return _mapper.Map<User>(user);
    }

    public async Task<User> Upsert(UserUpsert upsert)
    {
        var expression = (User x) => x.Id == upsert.Id;
        var entityUser = await _userRepository.FindByAsync(x => x.Id == upsert.Id);
        if (entityUser is null && upsert.Id is not null)
        {
            throw new NotFoundException("User not found");
        }

        entityUser = entityUser == null
            ? await InsertInternal(new Domain.Entities.User(), user => UpdateFromInternal(user, upsert))
            : await UpdateInternal(entityUser, user => UpdateFromInternal(user, upsert));

        await _unitOfWork.Commit();
        return _mapper.Map<User>(entityUser);
    }


    // TODO: Use Password Hashing Instead of Encryption
    /// <summary>
    /// For passwords, encryption is not recommended.
    /// ✅ Use a password hash algorithm (bcrypt, PBKDF2, Argon2).
    /// ✅ Store the salt with the hash.
    /// ✅ On password change, rehash and compare.
    /// </summary>
    /// <param name="command"></param>
    /// <see cref="https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html"/>
    /// <see cref="https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-8.0"/>
    public async Task<Result<User>> CreateUser(UserUpsert upsert)
    {
        var user = new Domain.Entities.User
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
        var normalizedEmail = email.ToLowerInvariant();
        var user = await _userRepository.FindByAsync(p => p.Email == normalizedEmail);

        return _mapper.Map<User>(user);
    }



    internal string DecryptPassword(Domain.Entities.User user)
    {
        var decryptedPassword = _generalDataProtectionService.DecryptString(user.Password);

        return decryptedPassword;
    }

    internal bool VerifyPassword(Domain.Entities.User user, string password)
    {
        var encryptedPassword = _generalDataProtectionService.EncryptString(password);

        return user.Password == password;
    }
}
