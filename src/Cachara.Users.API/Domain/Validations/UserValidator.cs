using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Domain.Errors;
using FluentValidation;

namespace Cachara.Users.API.Domain.Validations;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage(DomainErrors.User.EmailRequired.Message)
            .WithErrorCode(nameof(DomainErrors.User.EmailRequired))
            .MaximumLength(255).WithMessage("Email exceeds maximum length.")
            .WithErrorCode("EmailMaxLength")
            .EmailAddress().WithMessage(DomainErrors.User.InvalidEmail.Message)
            .WithErrorCode(nameof(DomainErrors.User.InvalidEmail))
            .Must(email => email == email.Trim()).WithMessage("Email cannot have leading or trailing spaces.")
            .WithErrorCode("EmailTrimmed");

        RuleFor(user => user.UserName)
            .NotEmpty().WithMessage(DomainErrors.User.UserNameRequired.Message)
            .WithErrorCode(nameof(DomainErrors.User.UserNameRequired))
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .WithErrorCode("UserNameMinLength")
            .MaximumLength(50).WithMessage("Username exceeds maximum length.")
            .WithErrorCode("UserNameMaxLength")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage(DomainErrors.User.InvalidUserName.Message)
            .WithErrorCode(nameof(DomainErrors.User.InvalidUserName));

        RuleFor(user => user.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .WithErrorCode("DateOfBirthRequired")
            .Must(date => date <= DateTime.UtcNow.AddYears(-18))
            .WithMessage(DomainErrors.User.InvalidDateOfBirth.Message)
            .WithErrorCode(nameof(DomainErrors.User.InvalidDateOfBirth))
            .Must(date => date >= DateTime.UtcNow.AddYears(-120))
            .WithMessage("Date of birth is too far in the past.")
            .WithErrorCode("DateOfBirthTooOld");

        RuleFor(user => user.FullName)
            .NotEmpty().WithMessage(DomainErrors.User.InvalidName.Message)
            .WithErrorCode(nameof(DomainErrors.User.InvalidName))
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .WithErrorCode("FullNameMinLength")
            .MaximumLength(100).WithMessage("Name exceeds maximum length.")
            .WithErrorCode("FullNameMaxLength")
            .Must(name => name == name.Trim()).WithMessage("Name cannot have leading or trailing spaces.")
            .WithErrorCode("FullNameTrimmed")
            .Must(name => !name.Contains("  ")).WithMessage("Name cannot contain consecutive spaces.")
            .WithErrorCode("FullNameConsecutiveSpaces");
    }
}
