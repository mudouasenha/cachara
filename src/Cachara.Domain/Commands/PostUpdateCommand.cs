namespace Cachara.Domain.Commands;

public class PostUpdateCommand
{
    public string Id { get; set; }
    
    public string Title { get; set; }

    public string Body { get; set; }
}