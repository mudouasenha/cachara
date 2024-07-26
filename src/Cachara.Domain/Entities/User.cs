using Cachara.Domain.Interfaces;

namespace Cachara.Domain.Entities;

public class User : IEntity<string>, IModifiable, IVersable, ISoftDeletable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public IEnumerable<Post> Posts { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public byte[] Version { get; set; }
    public bool Deleted { get; set; }
}