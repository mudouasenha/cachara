using Cachara.Shared.Domain.Entities.Abstractions;
using Cachara.Users.API.API.Security;
using FluentValidation.Results;

namespace Cachara.Users.API.Domain.Entities;



public class User : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Handle { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    // public Subscription Subscription { get; set; } TODO: think about it.
    public string ProfilePictureUrl { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public byte[] Version { get; set; }
    public bool Deleted { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserFollower> Followers { get; set; } = new List<UserFollower>();
    public ICollection<UserFollower> Following { get; set; } = new List<UserFollower>();
    
    public ValidationResult Validate()
    {
        throw new NotImplementedException();
    }

    public void ValidateAndThrow()
    {
        throw new NotImplementedException();
    }
}