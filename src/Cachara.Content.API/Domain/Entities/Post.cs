using Cachara.Shared.Domain.Entities.Abstractions;
using FluentValidation.Results;

namespace Cachara.Content.API.Domain.Entities;

public class Post : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
{
    public string Title { get; set; }

    public string Body { get; set; }

    public string AuthorId { get; set; }
    public string Id { get; set; }

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
