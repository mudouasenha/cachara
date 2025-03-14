using FluentResults;

namespace Cachara.Users.API.Domain.Errors;

public static class DomainErrors
{

    private const string ErrorCode = nameof(ErrorCode);


    public static class User
    {
        public static readonly IError EmailRequired = new Error("Email is required.")
            .WithMetadata(ErrorCode, nameof(EmailRequired));
        public static readonly IError InvalidEmail = new Error("Email is not a valid email address.")
            .WithMetadata(ErrorCode, nameof(EmailRequired));
        public static readonly IError UserNameRequired = new Error("User not found.")
            .WithMetadata(ErrorCode, nameof(UserNameRequired));
        public static readonly IError InvalidUserName = new Error("UserName is not valid.")
            .WithMetadata(ErrorCode, nameof(InvalidUserName));
        public static readonly IError InvalidDateOfBirth = new Error("Invalid date of birth.")
            .WithMetadata(ErrorCode, nameof(InvalidDateOfBirth));
        public static readonly IError InvalidName = new Error("Invalid name.")
            .WithMetadata(ErrorCode, nameof(InvalidName));
    }


}
