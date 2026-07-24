using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed record HandleStripeWebhookCommand(
    string Payload,
    string Signature) : ICommand<Result>;
