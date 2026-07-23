using ECommerce.Domain.Shared;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        Guid orderId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);

    /// <summary>Verify webhook signature and parse event type + paymentIntentId.</summary>
    Result<StripeWebhookEvent> ParseWebhook(string jsonBody, string stripeSignatureHeader);
}

public sealed record PaymentIntentResult(
    string PaymentIntentId,
    string ClientSecret,
    string PublishableKey);

public sealed record StripeWebhookEvent(
    string EventType,
    string PaymentIntentId);
