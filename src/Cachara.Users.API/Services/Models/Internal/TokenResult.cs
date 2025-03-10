namespace Cachara.Users.API.Services.Models.Internal;

public class TokenResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
