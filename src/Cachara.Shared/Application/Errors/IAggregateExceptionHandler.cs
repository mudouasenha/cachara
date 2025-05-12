using FluentResults;

namespace Cachara.Shared.Application.Errors;

public interface IAggregateExceptionHandler
{
    Result Handle(Exception exception);
}
