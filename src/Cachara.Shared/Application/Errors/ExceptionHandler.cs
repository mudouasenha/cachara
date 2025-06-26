using FluentResults;

namespace Cachara.Shared.Application.Errors;

public class ExceptionHandler : IErrorExceptionHandler<Exception>
{
    public IError Handle(Exception exception)
    {
        var error = new Error(exception.Message);
        return error;
    }
}


public class NotFoundExceptionHandler : IErrorExceptionHandler<NotFoundException>
{
    public IError Handle(NotFoundException exception)
    {
        var error = new Error(exception.Message);
        return error;
    }
}
