namespace Cachara.Users.API.Domain.Entities;

public class UserInterest
{
    public string Id { get; set; }
    public string UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }
    
    public string Interest { get; set; }
}