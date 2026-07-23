using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    ICurrentUserService currentUser,
    IBasketStore basketStore,
    IReadRepository<UserAddress> addressRepository,
    IRepository<DeliveryMethod> deliveryMethodRepository,
    IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<OrderResponse>.Failure(OrderErrors.Unauthorized);

        var userId = currentUser.UserId.Value;

        var address = await addressRepository.FirstOrDefaultAsync(
            new UserAddressByIdForUserSpecification(request.ShippingAddressId, userId),
            cancellationToken);

        if (address is null)
            return Result<OrderResponse>.Failure(OrderErrors.ShippingAddressNotFound);

        var deliveryMethod = await deliveryMethodRepository.GetByIdAsync(
            request.DeliveryMethodId,
            cancellationToken);

        if (deliveryMethod is null)
            return Result<OrderResponse>.Failure(DeliveryMethodErrors.NotFound);

        var basket = await basketStore.GetAsync(userId, cancellationToken);
        if (basket is null || basket.Items.Count == 0)
            return Result<OrderResponse>.Failure(OrderErrors.EmptyBasket);

        var lines = basket.Items
            .Select(i => (i.ProductId, i.ProductName, i.PictureUrl, i.UnitPrice, i.Quantity))
            .ToList();

        var createResult = Order.Create(
            Guid.NewGuid(),
            userId,
            deliveryMethod,
            address,
            lines);

        if (createResult.IsFailure)
            return Result<OrderResponse>.Failure(createResult.Error);

        var order = createResult.Value;
        orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        basket.Clear();
        await basketStore.SaveAsync(basket, cancellationToken);

        return Result<OrderResponse>.Success(OrderMappings.ToResponse(order));
    }
}
