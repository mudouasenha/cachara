using FluentValidation.Results;

namespace Cachara.Shared.Domain.Entities.Abstractions;

public interface IValidatable
{
    ValidationResult Validate();
    void ValidateAndThrow();
}
