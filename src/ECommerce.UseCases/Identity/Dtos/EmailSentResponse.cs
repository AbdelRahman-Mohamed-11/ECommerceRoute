namespace ECommerce.UseCases.Identity.Dtos;

/// <param name="Email">Address the verification code was sent to.</param>
/// <param name="VerificationCodeResent">
/// True when the email already belonged to an unconfirmed account and a fresh code was sent.
/// </param>
/// <param name="Message">User-facing success message for this outcome.</param>
public sealed record EmailSentResponse(
    string Email,
    bool VerificationCodeResent,
    string Message);
