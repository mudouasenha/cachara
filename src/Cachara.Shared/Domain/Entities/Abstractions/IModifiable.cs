namespace Cachara.Shared.Domain.Entities.Abstractions;

public interface IModifiable
{
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
