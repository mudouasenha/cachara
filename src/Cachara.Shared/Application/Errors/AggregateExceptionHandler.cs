﻿using System.Reflection;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Cachara.Shared.Application.Errors;

public class AggregateExceptionHandler : IAggregateExceptionHandler
{
    private readonly IServiceProvider serviceProvider;

    private readonly MethodInfo handleTypeMethod =
        typeof(AggregateExceptionHandler).GetMethod(
            nameof(HandleTyped),
            BindingFlags.NonPublic | BindingFlags.Instance);

    public AggregateExceptionHandler(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Result Handle(Exception exception)
    {
        var exceptionType = exception.GetType();

        var handleTypeMethodTyped = handleTypeMethod.MakeGenericMethod(exceptionType);
        var result = handleTypeMethodTyped.Invoke(this, [exception]);

        return (Result)result;
    }

    private Result HandleTyped<TException>(TException exception)
        where TException : Exception
    {
        var exceptionHandler = serviceProvider.GetService<IErrorExceptionHandler<TException>>();

        if (exceptionHandler is not null)
        {
            return new Result().WithError(exceptionHandler.Handle(exception));
        }

        return new Result().WithError(new ExceptionalError(exception));
    }
}
