using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using ECommerce.UseCases.Orders.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Infrastructure.Payments;

public sealed class StripePaymentService(
    IOptions<StripeSettings> stripeOptions,
    IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<StripePaymentService> logger) : IPaymentService
{
    private readonly StripeSettings _settings = stripeOptions.Value;

    public async Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit,
        string currency,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        if (amountInSmallestUnit < 1)
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var service = new PaymentIntentService();
        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"payment_intent_order_{orderId:D}"
        };

        try
        {
            var paymentIntent = await service.CreateAsync(
                new PaymentIntentCreateOptions
                {
                    Amount = amountInSmallestUnit,
                    Currency = currency.ToLowerInvariant(),
                    PaymentMethodTypes = ["card"],
                    Metadata = new Dictionary<string, string>
                    {
                        ["OrderId"] = orderId.ToString("D")
                    }
                },
                requestOptions,
                cancellationToken);

            logger.LogInformation(
                "Created PaymentIntent {Id} for Order {OrderId}",
                paymentIntent.Id, orderId);

            return ToResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe create failed for Order {OrderId}", orderId);
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }
    }

    public async Task<Result<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId,
        long amountInSmallestUnit,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        if (string.IsNullOrWhiteSpace(paymentIntentId))
            return Result<PaymentIntentResult>.Failure(OrderErrors.InvalidPaymentIntent);

        if (amountInSmallestUnit < 1)
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var service = new PaymentIntentService();
        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"payment_intent_order_{orderId:D}_update"
        };

        try
        {
            var paymentIntent = await service.UpdateAsync(
                paymentIntentId,
                new PaymentIntentUpdateOptions { Amount = amountInSmallestUnit },
                requestOptions,
                cancellationToken);

            logger.LogInformation(
                "Updated PaymentIntent {Id} for Order {OrderId}",
                paymentIntent.Id, orderId);

            return ToResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe update failed for Order {OrderId}", orderId);
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }
    }

    public async Task PaymentSucceeded(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FirstOrDefaultAsync(
            new OrderByPaymentIntentSpecification(paymentIntentId, tracking: true),
            cancellationToken);

        if (order is null)
        {
            logger.LogWarning(
                "PaymentSucceeded: no order for PaymentIntent {Id}", paymentIntentId);
            return;
        }

        var paid = order.MarkAsPaid(paymentIntentId);
        if (paid.IsFailure)
        {
            logger.LogWarning(
                "PaymentSucceeded MarkAsPaid failed for {OrderId}: {Error}",
                order.Id, paid.Error.Code);
            return;
        }

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Payment succeeded for Order {OrderId}, Intent {IntentId}",
            order.Id, paymentIntentId);
    }

    public Task PaymentFailed(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        logger.LogWarning(
            "Payment failed for PaymentIntent {Id}. Order stays Pending.",
            paymentIntentId);
        return Task.CompletedTask;
    }

    public Result<StripeWebhookEvent> ParseWebhook(string jsonPayload, string stripeSignature)
    {
        if (string.IsNullOrWhiteSpace(_settings.WebhookSecret)
            || string.IsNullOrWhiteSpace(jsonPayload)
            || string.IsNullOrWhiteSpace(stripeSignature))
            return Result<StripeWebhookEvent>.Failure(OrderErrors.InvalidWebhook);

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                jsonPayload,
                stripeSignature,
                _settings.WebhookSecret);

            logger.LogInformation(
                "Webhook received: {Type} | EventId: {Id}",
                stripeEvent.Type, stripeEvent.Id);

            var paymentIntentId = stripeEvent.Data.Object is PaymentIntent intent
                ? intent.Id
                : string.Empty;

            return Result<StripeWebhookEvent>.Success(new StripeWebhookEvent(
                stripeEvent.Type,
                paymentIntentId));
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Webhook signature verification failed.");
            return Result<StripeWebhookEvent>.Failure(OrderErrors.InvalidWebhook);
        }
    }

    private Result<PaymentIntentResult> ToResult(PaymentIntent paymentIntent)
    {
        if (string.IsNullOrWhiteSpace(paymentIntent.ClientSecret))
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        return Result<PaymentIntentResult>.Success(new PaymentIntentResult(
            paymentIntent.Id,
            paymentIntent.ClientSecret,
            paymentIntent.Status,
            _settings.PublishableKey));
    }
}
