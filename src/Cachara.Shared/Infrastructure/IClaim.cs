namespace Cachara.Shared.Infrastructure;

public interface IClaim
{
    public string Type { get; set; }
    public string Value { get; set; }
}