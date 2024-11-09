namespace Cachara.Users.API.Services.Abstractions;

public interface IUserFollowService
{
    Task Follow();
    Task Unfollow();
    Task GetFollowers();
    Task GetFollowing();
}