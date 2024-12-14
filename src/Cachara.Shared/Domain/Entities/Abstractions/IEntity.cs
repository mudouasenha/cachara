namespace Cachara.Shared.Domain.Entities.Abstractions;

public interface IEntity<TIdentifier>
{
    TIdentifier Id { get; set; }
}

