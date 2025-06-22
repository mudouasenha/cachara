namespace Cachara.Content.API.Services.Models;

public class Post
{
    public string Title { get; set; }

    public string Body { get; set; }

    public string AuthorId { get; set; }
    public string Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
