using Cachara.Shared.Domain.Errors;
using FluentResults;
using FluentValidation.Results;

namespace Cachara.Shared.Domain;

public static class ResultExtensions
{
    public static Result<T> WithErrorsFromValidationResult<T>(this Result<T> result, ValidationResult validationResult)
    {
        var errors = new List<Error>();
        errors.AddRange(validationResult.Errors.Select(p => new ValidationError(p.ErrorCode, p.ErrorMessage)));
        return result.WithErrors(errors);
    }
}
