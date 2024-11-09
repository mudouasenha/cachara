namespace Cachara.Users.API.Services;

public interface IUserFollowService
{
    Task Follow();
    Task Unfollow();
    Task GetFollowers();
    Task GetFollowing();
}