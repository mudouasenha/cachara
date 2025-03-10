using FluentResults;

namespace Cachara.Users.API.Domain.Errors;

public static class DomainErrors
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
    }


}
