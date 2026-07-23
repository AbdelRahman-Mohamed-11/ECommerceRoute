using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Identity;

/// <summary>
/// Teaching stub: does not send email. Logs the message (including the verification code)
/// to the console so you can copy it into confirm-email.
/// Student task: replace this with a real FluentEmail SMTP implementation.
/// </summary>
public sealed class NoOpEmailSender(ILogger<NoOpEmailSender> logger) : IEmailSender
{
    public Task<Result> SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        logger.LogWarning(
            "\n===== DEV: EMAIL NOT SENT (NoOpEmailSender) =====\n" +
            "To: {ToEmail}\n" +
            "Subject: {Subject}\n" +
            "{Body}\n" +
            "===============================================\n" +
            "Copy the verification code from the body above into POST /auth/confirm-email.",
            toEmail,
            subject,
            body);

        return Task.FromResult(Result.Success());
    }
}
