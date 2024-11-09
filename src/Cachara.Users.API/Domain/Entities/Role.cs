namespace Cachara.Users.API.Domain.Entities;

public class Role
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}