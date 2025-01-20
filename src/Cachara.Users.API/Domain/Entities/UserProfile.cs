using Cachara.Shared.Domain.Entities.Abstractions;
using FluentValidation.Results;

namespace Cachara.Users.API.Domain.Entities;

public class UserProfile : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public string UserId { get; set; }
    public User User { get; set; }

    public string Bio { get; set; }
    public string WebsiteUrl { get; set; }
    public string Location { get; set; }

    public ICollection<UserInterest> Interests { get; set; } = new List<UserInterest>();
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
