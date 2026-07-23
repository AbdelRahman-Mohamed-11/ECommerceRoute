using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class IdentityErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized(
            "Identity.InvalidCredentials",
            "Invalid email or password.");

    public static readonly Error EmailAlreadyExists =
        Error.Conflict(
            "Identity.EmailAlreadyExists",
            "Email is already registered.");

    public static readonly Error UserNotFound =
        Error.NotFound(
            "Identity.UserNotFound",
            "User was not found.");

    public static readonly Error EmailNotConfirmed =
        Error.Forbidden(
            "Identity.EmailNotConfirmed",
            "Email address has not been confirmed. Confirm your email before logging in.");

    public static readonly Error InvalidVerificationCode =
        Error.Validation(
            "Identity.InvalidVerificationCode",
            "Invalid or expired verification code.");

    public static readonly Error EmailAlreadyConfirmed =
        Error.Conflict(
            "Identity.EmailAlreadyConfirmed",
            "Email address is already confirmed.");

    public static readonly Error EmailSendFailed =
        Error.Failure(
            "Identity.EmailSendFailed",
            "Failed to send the verification email. Please try again later.");

    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized(
            "Identity.InvalidRefreshToken",
            "Invalid refresh token.");

    public static readonly Error RefreshTokenExpired =
        Error.Unauthorized(
            "Identity.RefreshTokenExpired",
            "Refresh token has expired. Please sign in again.");

    public static Error CreateFailed(string message) =>
        Error.Validation(
            "Identity.CreateFailed",
            message);

    public static Error Validation(string message) =>
        Error.Validation(
            "Identity.Validation",
            message);
}
