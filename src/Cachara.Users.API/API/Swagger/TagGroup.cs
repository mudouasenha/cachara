namespace Cachara.Users.API.API.Swagger
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TagGroup : Attribute
    {
        public string Group { get; set; }

        public TagGroup(string group)
        {
            Group = group;
        }
    }
}
