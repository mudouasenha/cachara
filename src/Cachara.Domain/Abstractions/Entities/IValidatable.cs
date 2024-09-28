using FluentValidation.Results;
namespace Cachara.Domain.Interfaces;

public interface IValidatable
{
    ValidationResult Validate();
    void ValidateAndThrow();
}