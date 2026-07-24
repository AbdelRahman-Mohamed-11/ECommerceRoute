using ECommerce.Domain.Shared;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit,
        string currency,
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<Result<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId,
        long amountInSmallestUnit,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>Called from webhook when payment_intent.succeeded.</summary>
    Task PaymentSucceeded(string paymentIntentId, CancellationToken cancellationToken = default);

    /// <summary>Called from webhook when payment_intent.payment_failed.</summary>
    Task PaymentFailed(string paymentIntentId, CancellationToken cancellationToken = default);
}

public sealed record PaymentIntentResult(
    string PaymentIntentId,
    string ClientSecret,
    string Status,
    string PublishableKey);
