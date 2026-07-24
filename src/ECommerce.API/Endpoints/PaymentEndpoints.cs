using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.API.Endpoints;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/payments")
            .WithTags("Payments")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0));

        // Public — Stripe calls this (no JWT)
        group.MapPost("/webhook", async (
            HttpRequest request,
            IPaymentService paymentService,
            IOptions<StripeSettings> stripeOptions,
            CancellationToken ct) =>
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync(ct);
            var signature = request.Headers["Stripe-Signature"].ToString();

            if (string.IsNullOrWhiteSpace(signature))
                return Results.BadRequest("Missing Stripe-Signature header.");

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    stripeOptions.Value.WebhookSecret);

                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                    {
                        var succeeded = stripeEvent.Data.Object as PaymentIntent;
                        if (succeeded is not null)
                            await paymentService.PaymentSucceeded(succeeded.Id, ct);
                        break;
                    }

                    case EventTypes.PaymentIntentPaymentFailed:
                    {
                        var failed = stripeEvent.Data.Object as PaymentIntent;
                        if (failed is not null)
                            await paymentService.PaymentFailed(failed.Id, ct);
                        break;
                    }

                    default:
                        break;
                }

                return Results.Ok();
            }
            catch (StripeException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .AllowAnonymous()
        .WithSummary("Stripe webhook")
        .WithDescription("Verifies Stripe-Signature; calls PaymentSucceeded / PaymentFailed on the payment service.");

        return endpoints;
    }
}
