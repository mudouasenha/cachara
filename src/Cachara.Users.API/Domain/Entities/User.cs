using Cachara.Shared.Domain;
using Cachara.Shared.Domain.Entities.Abstractions;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.Domain.Validations;
using FluentValidation;
using FluentValidation.Results;

namespace Cachara.Users.API.Domain.Entities;

public class User : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public Subscription Subscription { get; set; }
    // TODO: Use a storage functionality to store the data, and change
    // it to a file reference
    //public string ProfilePictureUrl { get; set; }

    public UserSettings Settings { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void AssignRole(RoleType role)
    {
        if (UserRoles.Any(p => p.RoleId == role))
        {
            throw new Exception("User has already assigned role");
        }

        var newRole = new UserRole()
        {
            UserId = Id,
            AssignedDate = DateTimeOffset.UtcNow,
            RoleId = role
        };
        newRole.GenerateId();
        newRole.UpdateCreatedAt();
        newRole.UpdateUpdateAt();

        UserRoles.Add(newRole);
    }

    public bool Deleted { get; set; }
    //public ICollection<UserFollower> Followers { get; set; } = new List<UserFollower>();
    //public ICollection<UserFollower> Following { get; set; } = new List<UserFollower>();

    public Task<ValidationResult> Validate()
        => Task.FromResult(new UserValidator().Validate(this));

    public Task ValidateAndThrow()
    {
        new UserValidator().ValidateAndThrow(this);
        return Task.CompletedTask;
    }

    public byte[] Version { get; set; }
}
