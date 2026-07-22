using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using FluentEmail.Core;

namespace ECommerce.Infrastructure.Identity;

public sealed class FluentEmailSender(IFluentEmailFactory emailFactory) : IEmailSender
{
    public async Task<Result> SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;

        var response = await emailFactory
            .Create()
            .To(toEmail)
            .Subject(subject)
            .Body(body)
            .SendAsync();

        if (!response.Successful)
            return Result.Failure(IdentityErrors.EmailSendFailed);

        return Result.Success();
    }
}
