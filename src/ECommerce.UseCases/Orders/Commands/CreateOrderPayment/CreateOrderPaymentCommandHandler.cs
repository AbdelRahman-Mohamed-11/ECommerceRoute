using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Specifications;
using Microsoft.Extensions.Options;

namespace ECommerce.UseCases.Orders.Commands.CreateOrderPayment;

public sealed class CreateOrderPaymentCommandHandler(
    ICurrentUserService currentUser,
    IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork,
    IPaymentService paymentService,
    IOptions<StripeSettings> stripeOptions)
    : IRequestHandler<CreateOrderPaymentCommand, Result<PaymentClientSecretResponse>>
{
    public async Task<Result<PaymentClientSecretResponse>> Handle(
        CreateOrderPaymentCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<PaymentClientSecretResponse>.Failure(OrderErrors.Unauthorized);

        var order = await orderRepository.FirstOrDefaultAsync(
            new OrderByIdForUserSpecification(request.OrderId, currentUser.UserId.Value, tracking: true),
            cancellationToken);

        if (order is null)
            return Result<PaymentClientSecretResponse>.Failure(OrderErrors.NotFound);

        if (order.Status != OrderStatus.Pending)
            return Result<PaymentClientSecretResponse>.Failure(OrderErrors.InvalidPaymentState);

        var currency = stripeOptions.Value.Currency;
        var amountInSmallestUnit = (long)Math.Round(
            order.Total * 100m, MidpointRounding.AwayFromZero);

        Result<PaymentIntentResult> intentResult;

        if (string.IsNullOrWhiteSpace(order.PaymentIntentId))
        {
            intentResult = await paymentService.CreatePaymentIntentAsync(
                amountInSmallestUnit,
                currency,
                order.Id,
                cancellationToken);
        }
        else
        {
            intentResult = await paymentService.UpdatePaymentIntentAsync(
                order.PaymentIntentId,
                amountInSmallestUnit,
                order.Id,
                cancellationToken);
        }

        if (intentResult.IsFailure)
            return Result<PaymentClientSecretResponse>.Failure(intentResult.Error);

        var attach = order.AttachPaymentIntent(intentResult.Value.PaymentIntentId);
        if (attach.IsFailure)
            return Result<PaymentClientSecretResponse>.Failure(attach.Error);

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var value = intentResult.Value;
        return Result<PaymentClientSecretResponse>.Success(
            new PaymentClientSecretResponse(
                order.Id,
                value.PaymentIntentId,
                value.ClientSecret,
                value.Status,
                value.PublishableKey));
    }
}
