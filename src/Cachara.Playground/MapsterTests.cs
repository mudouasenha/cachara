using Mapster;

namespace Cachara.Playground;

public class UserDto
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
}

public class User
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
}

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<UserDto, User>.NewConfig()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
    }
}


public class MapsterTests
{
    public void TestMapster()
    {
        UserDto dto = new UserDto
        {
            FirstName = "Matheus",
            LastName = "Torres",
            Email = "matheus@example.com"
        };

        User user = dto.Adapt<User>();

        Console.WriteLine(user.FullName);
    }
}
