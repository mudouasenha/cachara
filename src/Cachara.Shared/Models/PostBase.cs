namespace Cachara.Core.Models;

public class PostBase
{
    public string Id { get; set; }
    
    public string Title { get; set; }

    public string Body { get; set; }

    public string AuthorName { get; set; }
        
    public string AuthorUserName { get; set; }
        
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}