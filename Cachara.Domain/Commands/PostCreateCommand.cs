namespace Cachara.Domain.Commands;

public class PostCreateCommand
{
    public string Title { get; set; }

    public string Body { get; set; }

    public string Author { get; set; }

    public int Reads { get; set; }

    public bool Deleted { get; set; }
}