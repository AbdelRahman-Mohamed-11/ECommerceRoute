using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed class HandleStripeWebhookCommandHandler(
    IPaymentService paymentService)
    : IRequestHandler<HandleStripeWebhookCommand, Result>
{
    // Stripe event type strings (same values as Stripe.EventTypes).
    private const string PaymentIntentSucceeded = "payment_intent.succeeded";
    private const string PaymentIntentPaymentFailed = "payment_intent.payment_failed";

    public async Task<Result> Handle(
        HandleStripeWebhookCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Signature))
            return Result.Failure(OrderErrors.InvalidWebhook);

        var parsed = paymentService.ParseWebhook(request.Payload, request.Signature);
        if (parsed.IsFailure)
            return Result.Failure(parsed.Error);

        var stripeEvent = parsed.Value;

        switch (stripeEvent.EventType)
        {
            case PaymentIntentSucceeded:
                if (!string.IsNullOrWhiteSpace(stripeEvent.PaymentIntentId))
                    await paymentService.PaymentSucceeded(stripeEvent.PaymentIntentId, cancellationToken);
                break;

            case PaymentIntentPaymentFailed:
                if (!string.IsNullOrWhiteSpace(stripeEvent.PaymentIntentId))
                    await paymentService.PaymentFailed(stripeEvent.PaymentIntentId, cancellationToken);
                break;
        }

        return Result.Success();
    }
}
