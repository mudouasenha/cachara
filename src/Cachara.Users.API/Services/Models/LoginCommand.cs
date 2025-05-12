namespace Cachara.Users.API.Services.Models;

public record LoginCommand(string Email, string Password);

public record RegisterCommand(string UserName, string Email, DateOnly DateOfBirth, string FullName, string Password);
