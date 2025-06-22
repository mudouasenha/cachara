namespace Cachara.Content.API.Domain.Commands;

public class PostSearchCommand
{
    public string Title { get; set; }

    public string SearchString { get; set; }

    public string Author { get; set; }

    public int Views { get; set; }

    public bool Deleted { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
