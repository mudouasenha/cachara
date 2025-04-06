using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cachara.Shared.Application;

public class GlobalExceptionHandler : IExceptionHandler<Exception>
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }


    public ValueTask<Result> Handle(Exception ex)
    {
        _logger.LogError(ex, "Exception Occurred: {Message}", ex.Message);
        throw new NotImplementedException("Implement global exception handler");
        return new ValueTask<Result>(Result.Fail("Exception Occurred").WithError(ex.Message));
    }
}

public interface IExceptionHandler<TException> where TException : Exception
{
    ValueTask<Result> Handle(Exception ex);
}
