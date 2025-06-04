using FluentResults;

namespace Cachara.Users.API.Services.Errors;

public class ApplicationErrors
{

    private const string ErrorCode = nameof(ErrorCode);

    public static class UserAuthentication
    {
        public static readonly IError InvalidCredentials = new Error("Invalid Credentials.")
            .WithMetadata(ErrorCode, nameof(InvalidCredentials));
        public static readonly IError UserNotFound = new Error("User not found.")
            .WithMetadata(ErrorCode, nameof(UserNotFound));
        public static readonly IError UnsafePassword = new Error("New password does not meet security requirements.")
            .WithMetadata(ErrorCode, nameof(UnsafePassword));
        public static readonly IError SamePassword = new Error("New password cannot be the same as the current password.")
            .WithMetadata(ErrorCode, nameof(SamePassword));
        public static readonly IError UserNameAlreadyExists = new Error("UserName already exists.")
            .WithMetadata(ErrorCode, nameof(UserNameAlreadyExists));
        public static readonly IError UserEmailAlreadyExists = new Error("User email already exists.")
            .WithMetadata(ErrorCode, nameof(UserEmailAlreadyExists));
    }

    public static class Password
    {
        public static readonly IError Required = new Error("Password is required.")
            .WithMetadata(ErrorCode, nameof(Required));

        public static readonly IError MinimumLength = new Error("Password must be at least 8 characters.")
            .WithMetadata(ErrorCode, nameof(MinimumLength));

        public static readonly IError MaximumLength = new Error("Password cannot exceed 50 characters.")
            .WithMetadata(ErrorCode, nameof(MaximumLength));

        public static readonly IError UppercaseRequired = new Error("Password must contain at least one uppercase letter.")
            .WithMetadata(ErrorCode, nameof(UppercaseRequired));

        public static readonly IError LowercaseRequired = new Error("Password must contain at least one lowercase letter.")
            .WithMetadata(ErrorCode, nameof(LowercaseRequired));

        public static readonly IError NumberRequired = new Error("Password must contain at least one number.")
            .WithMetadata(ErrorCode, nameof(NumberRequired));

        public static readonly IError SpecialCharacterRequired = new Error("Password must contain at least one special character.")
            .WithMetadata(ErrorCode, nameof(SpecialCharacterRequired));
    }
}
