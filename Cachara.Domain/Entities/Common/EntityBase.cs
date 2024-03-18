namespace Cachara.Domain.Entities.Common
{
    public abstract class EntityBase
    {
        public string Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
