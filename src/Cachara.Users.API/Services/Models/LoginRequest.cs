namespace Cachara.Users.API.Services.Models;

public record LoginRequest(string Email, string Password);

public record ChangePasswordRequest(string Password, string NewPassword);
public record RegisterRequest(string UserName, string Email, DateOnly DateOfBirth, string FullName, string Password);
