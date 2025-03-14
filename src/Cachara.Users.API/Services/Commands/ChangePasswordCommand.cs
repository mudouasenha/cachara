namespace Cachara.Users.API.Services.Commands;

public class ChangePasswordCommand
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}
