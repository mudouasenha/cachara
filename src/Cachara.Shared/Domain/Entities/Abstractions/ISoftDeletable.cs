namespace Cachara.Shared.Domain.Entities.Abstractions;

public interface ISoftDeletable
{
    bool Deleted { get; set; }
}
