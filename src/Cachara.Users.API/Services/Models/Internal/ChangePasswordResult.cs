namespace Cachara.Users.API.Services.Models.Internal;

public class ChangePasswordResult
{
    public string Message { get; set; }
    public DateTimeOffset LastChanged { get; set; } // TODO: Consider offset on result
}
