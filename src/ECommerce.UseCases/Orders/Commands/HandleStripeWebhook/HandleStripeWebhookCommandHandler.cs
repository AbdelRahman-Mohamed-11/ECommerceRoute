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
    public async Task<Result> Handle(
        HandleStripeWebhookCommand request,
        CancellationToken cancellationToken)
    {
        var parsed = paymentService.ParseWebhook(request.JsonBody, request.StripeSignatureHeader);
        if (parsed.IsFailure)
            return Result.Failure(parsed.Error);

        var evt = parsed.Value;
        if (string.IsNullOrWhiteSpace(evt.PaymentIntentId)
            || evt.EventType is not ("payment_intent.succeeded" or "payment_intent.payment_failed"))
            return Result.Success();

        var order = await orderRepository.FirstOrDefaultAsync(
            new OrderByPaymentIntentSpecification(evt.PaymentIntentId, tracking: true),
            cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (evt.EventType == "payment_intent.succeeded")
        {
            var paid = order.MarkAsPaid(evt.PaymentIntentId);
            if (paid.IsFailure)
                return paid;

            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
