namespace ECommerce.UseCases.Common.Interfaces;

public interface IEmailVerificationCodeStore
{
    Task SaveAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true when the code matches and removes it (one-time use).
    /// </summary>
    Task<bool> ValidateAndConsumeAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default);
}
