namespace Cachara.Domain.Commands;

public class PostCreateCommand
{
    public string Title { get; set; }

    public string Body { get; set; }

    public string AuthorId { get; set; }
}