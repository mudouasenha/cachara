namespace Cachara.Shared.Application.Abstractions;

public interface IClaim
{
    public string Type { get; set; }
    public string Value { get; set; }
}