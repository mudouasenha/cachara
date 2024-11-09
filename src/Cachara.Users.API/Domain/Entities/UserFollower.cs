namespace Cachara.Users.API.Domain.Entities;

public class UserFollower
{
    public string UserId { get; set; }
    public User User { get; set; } // The user being followed
    
    public string FollowerId { get; set; }
    public User Follower { get; set; } // The user following
    
    public DateTimeOffset FollowedAt { get; set; }
}