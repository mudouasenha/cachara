using FluentValidation.Results;

namespace Cachara.Shared.Domain.Entities.Abstractions;

public interface IValidatable
{
    Task<ValidationResult> Validate();
    Task ValidateAndThrow();
}
