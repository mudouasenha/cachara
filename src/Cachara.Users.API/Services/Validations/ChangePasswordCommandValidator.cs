using Cachara.Users.API.Domain.Errors;
using Cachara.Users.API.Services.Commands;
using FluentValidation;

namespace Cachara.Users.API.Services.Validations;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x)
            .Must(BeDifferentPasswords)
            .WithMessage(DomainErrors.UserAuthentication.SamePassword.Message)
            .WithErrorCode(nameof(DomainErrors.UserAuthentication.SamePassword));
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(DomainErrors.Password.Required.Message)
            .WithErrorCode(nameof(DomainErrors.Password.Required))
            .MinimumLength(8).WithMessage(DomainErrors.Password.MinimumLength.Message)
            .WithErrorCode(nameof(DomainErrors.Password.MinimumLength))
            .MaximumLength(50).WithMessage(DomainErrors.Password.MaximumLength.Message)
            .WithErrorCode(nameof(DomainErrors.Password.MaximumLength))
            .Matches("[A-Z]").WithMessage(DomainErrors.Password.UppercaseRequired.Message)
            .WithErrorCode(nameof(DomainErrors.Password.UppercaseRequired))
            .Matches("[a-z]").WithMessage(DomainErrors.Password.LowercaseRequired.Message)
            .WithErrorCode(nameof(DomainErrors.Password.LowercaseRequired))
            .Matches("[0-9]").WithMessage(DomainErrors.Password.NumberRequired.Message)
            .WithErrorCode(nameof(DomainErrors.Password.NumberRequired))
            .Matches("[^a-zA-Z0-9]").WithMessage(DomainErrors.Password.SpecialCharacterRequired.Message)
            .WithErrorCode(nameof(DomainErrors.Password.SpecialCharacterRequired));
    }

    private static bool BeDifferentPasswords(ChangePasswordCommand command)
    {
        return command.NewPassword != command.Password;
    }
}
