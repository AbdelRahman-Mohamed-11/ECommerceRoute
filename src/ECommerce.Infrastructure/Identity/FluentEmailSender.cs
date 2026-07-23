using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Identity;

/// <summary>
/// Dev stub: logs the email (including verification code) to the console.
/// Later: inject <c>IFluentEmailFactory</c> and call FluentEmail SMTP send here.
/// </summary>
public sealed class FluentEmailSender(ILogger<FluentEmailSender> logger) : IEmailSender
{
    public Task<Result> SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;

        logger.LogWarning(
            "\n===== DEV: EMAIL NOT SENT (FluentEmailSender) =====\n" +
            "To: {ToEmail}\n" +
            "Subject: {Subject}\n" +
            "{Body}\n" +
            "=================================================\n" +
            "Copy the verification code from the body above into POST /auth/confirm-email.",
            toEmail,
            subject,
            body);

        return Task.FromResult(Result.Success());
    }
}
