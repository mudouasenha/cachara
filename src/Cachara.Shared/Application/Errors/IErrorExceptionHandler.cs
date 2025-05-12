using FluentResults;

namespace Cachara.Shared.Application.Errors;

public interface IErrorExceptionHandler<TException>
    where TException : Exception
{
    IError Handle(TException exception);
}
