using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed record HandleStripeWebhookCommand(
    string JsonBody,
    string StripeSignatureHeader) : ICommand<Result>;
