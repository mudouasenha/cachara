using Cachara.Shared.Domain.Entities.Abstractions;
using FluentValidation.Results;

namespace Cachara.Users.API.Domain.Entities;

public class UserSettings : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    
    public bool IsPrivate { get; set; }
    public bool ReceiveNotifications { get; set; }
    public bool ShowEmail { get; set; }
    
        
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public byte[] Version { get; set; }
    public bool Deleted { get; set; }
    
    public ValidationResult Validate()
    {
        throw new NotImplementedException();
    }

    public void ValidateAndThrow()
    {
        throw new NotImplementedException();
    }
}