using ECommerce.Domain.Shared;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IEmailSender
{
    Task<Result> SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}
