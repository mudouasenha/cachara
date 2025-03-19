namespace Cachara.Users.API.Services.Models.Internal;

public class UserLoginResult
{
    public TokenResult Token { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
}
