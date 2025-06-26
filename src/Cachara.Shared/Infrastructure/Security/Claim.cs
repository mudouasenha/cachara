using Cachara.Shared.Application.Abstractions;

namespace Cachara.Shared.Infrastructure.Security;

public class Claim : IClaim
{
    public string Type { get; set; }
    public string Value { get; set; }

    public Claim(string type, string value)
    {
        Type = type;
        Value = value;
    }

    public static Claim FromClaim(System.Security.Claims.Claim claim)
    {
        return new Claim(claim.Type, claim.Value);
    }
}
