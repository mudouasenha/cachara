using Cachara.Domain.Interfaces;
using FluentValidation.Results;

namespace Cachara.Users.API.Domain.Entities;



public class User : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
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