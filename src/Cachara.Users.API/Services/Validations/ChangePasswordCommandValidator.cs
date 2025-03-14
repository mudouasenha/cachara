using Cachara.Users.API.Services.Commands;
using Cachara.Users.API.Services.Errors;
using FluentValidation;

namespace Cachara.Users.API.Services.Validations;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(ApplicationErrors.Password.Required.Message)
            .WithErrorCode(nameof(ApplicationErrors.Password.Required))
            .MinimumLength(8).WithMessage(ApplicationErrors.Password.MinimumLength.Message)
            .WithErrorCode(nameof(ApplicationErrors.Password.MinimumLength))
            .MaximumLength(50).WithMessage(ApplicationErrors.Password.MaximumLength.Message)
            .WithErrorCode(nameof(ApplicationErrors.Password.MaximumLength))
            .Matches("[A-Z]").WithMessage(ApplicationErrors.Password.UppercaseRequired.Message)
            .WithErrorCode(nameof(ApplicationErrors.Password.UppercaseRequired))
            .Matches("[a-z]").WithMessage(ApplicationErrors.Password.LowercaseRequired.Message)
            .WithErrorCode(nameof(ApplicationErrors.Password.LowercaseRequired))
            .Matches("[0-9]").WithMessage(ApplicationErrors.Password.NumberRequired.Message)
            .WithErrorCode(nameof(ApplicationErrors.Password.NumberRequired))
            .Matches("[^a-zA-Z0-9]").WithMessage(ApplicationErrors.Password.SpecialCharacterRequired.Message)
            .WithErrorCode(nameof(ApplicationErrors.Password.SpecialCharacterRequired));
    }
}
