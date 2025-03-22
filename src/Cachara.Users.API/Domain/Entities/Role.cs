using Cachara.Shared.Domain.Entities.Abstractions;
using FluentValidation.Results;

namespace Cachara.Users.API.Domain.Entities;

public class Role : IEntity<RoleType>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public RoleType Id { get; set; }
    public string Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool Deleted { get; set; }

    public Task<ValidationResult> Validate()
    {
        throw new NotImplementedException();
    }

    public Task ValidateAndThrow()
    {
        throw new NotImplementedException();
    }

    public byte[] Version { get; set; }
}

public enum RoleType
{
    NotSet,
    User,
    Admin
}
