using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

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

        group.MapPost("/webhook", async (
            HttpRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync(ct);
            var signature = request.Headers["Stripe-Signature"].ToString();

            var result = await sender.Send(
                new HandleStripeWebhookCommand(json, signature),
                ct);

            return result.IsFailure
                ? Results.BadRequest()
                : Results.Ok();
        })
        .AllowAnonymous()
        .WithSummary("Stripe webhook")
        .WithDescription("Verifies Stripe-Signature and marks orders paid on payment_intent.succeeded.");

        return endpoints;
    }
}
