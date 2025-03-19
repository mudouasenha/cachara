namespace Cachara.Users.API.Services.Commands;

public class ChangePasswordCommand
{
    public string Password { get; set; }
    public string NewPassword { get; set; }
}
