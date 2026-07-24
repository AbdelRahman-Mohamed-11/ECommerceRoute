using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed class HandleStripeWebhookCommandHandler(
    IPaymentService paymentService,
    IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<HandleStripeWebhookCommand, Result>
{
    private const string PaymentIntentSucceeded = "payment_intent.succeeded";
    private const string PaymentIntentFailed = "payment_intent.payment_failed";
    private const string PaymentIntentRequiresAction = "payment_intent.requires_action";

    public async Task<Result> Handle(
        HandleStripeWebhookCommand request,
        CancellationToken cancellationToken)
    {
        var parsed = paymentService.ParseWebhook(
            request.JsonBody, request.StripeSignatureHeader);

        if (parsed.IsFailure)
            return Result.Failure(parsed.Error);

        var evt = parsed.Value;

        if (string.IsNullOrWhiteSpace(evt.PaymentIntentId))
            return Result.Success();

        if (evt.EventType is PaymentIntentRequiresAction or PaymentIntentFailed)
            return Result.Success();

        if (evt.EventType != PaymentIntentSucceeded)
            return Result.Success();

        var order = await orderRepository.FirstOrDefaultAsync(
            new OrderByPaymentIntentSpecification(evt.PaymentIntentId, tracking: true),
            cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        var paid = order.MarkAsPaid(evt.PaymentIntentId);
        if (paid.IsFailure)
            return paid;

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
