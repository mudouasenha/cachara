namespace Cachara.Domain.Interfaces;

public interface IModifiable
{
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}