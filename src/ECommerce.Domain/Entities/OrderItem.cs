using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public sealed class OrderItem
{
    private OrderItem()
    {
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public ProductItemOrdered ItemOrdered { get; private set; } = null!;
    public int Quantity { get; private set; }

    public decimal LineTotal => ItemOrdered.UnitPrice * Quantity;

    internal static Result<OrderItem> Create(
        Guid id,
        ProductItemOrdered itemOrdered,
        int quantity)
    {
        if (id == Guid.Empty)
            return Result<OrderItem>.Failure(OrderErrors.InvalidItemId);

        if (itemOrdered is null)
            return Result<OrderItem>.Failure(OrderErrors.InvalidProductId);

        if (quantity < 1)
            return Result<OrderItem>.Failure(OrderErrors.InvalidQuantity);

        return Result<OrderItem>.Success(new OrderItem
        {
            Id = id,
            ItemOrdered = itemOrdered,
            Quantity = quantity
        });
    }

    internal void AssignOrder(Guid orderId) => OrderId = orderId;
}
