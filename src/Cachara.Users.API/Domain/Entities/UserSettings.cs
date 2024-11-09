namespace Cachara.Users.API.Domain.Entities;

public class UserSettings
{
    public string UserId { get; set; }
    public User User { get; set; }
    
    public bool IsPrivate { get; set; }
    public bool ReceiveNotifications { get; set; }
    public bool ShowEmail { get; set; }
}