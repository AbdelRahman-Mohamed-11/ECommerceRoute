using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Dtos;

namespace ECommerce.UseCases.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid ShippingAddressId,
    Guid DeliveryMethodId) : ICommand<Result<OrderResponse>>;
