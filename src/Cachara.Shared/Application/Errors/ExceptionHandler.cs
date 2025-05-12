using FluentResults;

namespace Cachara.Shared.Application.Errors;

public class ExceptionHandler : IErrorExceptionHandler<Exception>
{
    public IError Handle(Exception exception)
    {
        var error = new Error(exception.Message);
        return error;
    }

    public IError Handle<TResult>(Exception exception) where TResult : Result
    {
        var error = new Error(exception.Message);
        return error;
    }
}
