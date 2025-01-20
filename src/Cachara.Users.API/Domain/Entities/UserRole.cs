using Cachara.Shared.Domain.Entities.Abstractions;
using FluentValidation.Results;

namespace Cachara.Users.API.Domain.Entities;

public class UserRole : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public string UserId { get; set; }
    public User User { get; set; }

    public string RoleId { get; set; }
    public Role Role { get; set; }

    public DateTime AssignedDate { get; set; }
    public string Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool Deleted { get; set; }

    public ValidationResult Validate()
    {
        throw new NotImplementedException();
    }

    public void ValidateAndThrow()
    {
        throw new NotImplementedException();
    }

    public byte[] Version { get; set; }
}
