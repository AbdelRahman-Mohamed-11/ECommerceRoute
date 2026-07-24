using ECommerce.Domain.Shared;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IPaymentService
{
    /// <summary>Create a new PaymentIntent for the order (Idempotency-Key scoped to orderId).</summary>
    Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit,
        string currency,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>Update the amount on an existing PaymentIntent (e.g. before the customer pays).</summary>
    Task<Result<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId,
        long amountInSmallestUnit,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>Verify Stripe-Signature and map to a typed webhook event.</summary>
    Result<StripeWebhookEvent> ParseWebhook(string jsonPayload, string stripeSignature);
}

public sealed record PaymentIntentResult(
    string PaymentIntentId,
    string ClientSecret,
    string Status,
    string PublishableKey);

public sealed record StripeWebhookEvent(
    string EventType,
    string PaymentIntentId,
    string? OrderIdFromMetadata,
    string? FailureMessage);
