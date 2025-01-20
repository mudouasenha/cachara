using Cachara.Users.API.API.Security;

namespace Cachara.Users.API.Services.Models;

public class UserUpsert
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Subscription Subscription { get; set; }
}
