namespace Cachara.Users.API.API.Swagger;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class TagGroup : Attribute
{
    public TagGroup(string group)
    {
        Group = group;
    }

    public string Group { get; set; }
}
