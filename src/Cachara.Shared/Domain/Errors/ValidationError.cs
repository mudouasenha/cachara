using FluentResults;

namespace Cachara.Shared.Domain.Errors;

public class ValidationError : Error
{
    public ValidationError(string errorCode, string errorMessage) : base(errorMessage)
    {
        Metadata.Add("ErrorCode", errorCode);
    }

    public override string ToString()
    {
        return "A validation error have occurred";
    }
}
