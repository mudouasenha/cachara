namespace Cachara.Domain.Commands;

public class PostUpsert
{
    public string Id { get; set; }
    public string Title { get; set; }

    public string Body { get; set; }

    public string AuthorId { get; set; }
}