namespace Cachara.Users.API.Services.Models.Internal;

public class UserRegisterResult
{
    public TokenResult Token { get; set; }
    public string SessionId { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}
