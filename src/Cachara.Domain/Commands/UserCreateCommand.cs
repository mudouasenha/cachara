namespace Cachara.Domain.Commands;

public class UserCreateCommand
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}