namespace Cachara.Users.API.Domain.Entities;

public class UserProfile
{
    public string UserId { get; set; }
    public User User { get; set; }
    
    public string Bio { get; set; }
    public string WebsiteUrl { get; set; }
    public string Location { get; set; }
    
    public ICollection<UserInterest> Interests { get; set; } = new List<UserInterest>();
}