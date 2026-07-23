using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Infrastructure.Payments;

public sealed class StripePaymentService(IOptions<StripeSettings> options) : IPaymentService
{
    private readonly StripeSettings _settings = options.Value;

    public async Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        Guid orderId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var amountInMinorUnits = (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
        if (amountInMinorUnits < 1)
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        var service = new PaymentIntentService();
        try
        {
            var intent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = amountInMinorUnits,
                Currency = currency.ToLowerInvariant(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = orderId.ToString()
                }
            }, cancellationToken: cancellationToken);

            if (string.IsNullOrWhiteSpace(intent.ClientSecret))
                return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

            return Result<PaymentIntentResult>.Success(new PaymentIntentResult(
                intent.Id,
                intent.ClientSecret,
                _settings.PublishableKey));
        }
        catch (StripeException)
        {
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }
    }

    public Result<StripeWebhookEvent> ParseWebhook(string jsonBody, string stripeSignatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_settings.WebhookSecret)
            || string.IsNullOrWhiteSpace(jsonBody)
            || string.IsNullOrWhiteSpace(stripeSignatureHeader))
            return Result<StripeWebhookEvent>.Failure(OrderErrors.InvalidWebhook);

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                jsonBody,
                stripeSignatureHeader,
                _settings.WebhookSecret);

            if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
                return Result<StripeWebhookEvent>.Success(new StripeWebhookEvent(
                    stripeEvent.Type,
                    string.Empty));

            return Result<StripeWebhookEvent>.Success(new StripeWebhookEvent(
                stripeEvent.Type,
                paymentIntent.Id));
        }
        catch (StripeException)
        {
            return Result<StripeWebhookEvent>.Failure(OrderErrors.InvalidWebhook);
        }
    }
}
