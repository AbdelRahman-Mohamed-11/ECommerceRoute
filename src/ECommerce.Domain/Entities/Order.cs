using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public sealed class Order : BaseEntity
{
    public const int MaxNameLength = 100;
    public const int MaxPhoneLength = 32;
    public const int MaxCountryLength = 100;
    public const int MaxCityLength = 100;
    public const int MaxStreetLength = 200;
    public const int MaxPostalCodeLength = 20;
    public const int MaxDeliveryMethodNameLength = 100;
    public const int MaxDeliveryTimeLength = 100;

    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }

    public Guid DeliveryMethodId { get; private set; }
    public string DeliveryMethodName { get; private set; } = null!;
    public decimal DeliveryMethodPrice { get; private set; }
    public string DeliveryMethodEstimatedTime { get; private set; } = null!;

    public string ShippingRecipientFirstName { get; private set; } = null!;
    public string ShippingRecipientLastName { get; private set; } = null!;
    public string ShippingPhoneNumber { get; private set; } = null!;
    public string ShippingCountry { get; private set; } = null!;
    public string ShippingCity { get; private set; } = null!;
    public string ShippingStreet { get; private set; } = null!;
    public string ShippingPostalCode { get; private set; } = null!;

    public decimal SubTotal { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal Total { get; private set; }

    public string? PaymentIntentId { get; private set; }
    public DateTimeOffset? PaidAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items;

    public static Result<Order> Create(
        Guid id,
        Guid userId,
        DeliveryMethod deliveryMethod,
        UserAddress shippingAddress,
        IReadOnlyList<(Guid ProductId, string ProductName, string PictureUrl, decimal UnitPrice, int Quantity)> basketItems)
    {
        if (id == Guid.Empty)
            return Result<Order>.Failure(OrderErrors.InvalidId);

        if (userId == Guid.Empty)
            return Result<Order>.Failure(OrderErrors.InvalidUserId);

        if (deliveryMethod is null)
            return Result<Order>.Failure(OrderErrors.DeliveryMethodRequired);

        if (!deliveryMethod.IsAvailable)
            return Result<Order>.Failure(OrderErrors.DeliveryMethodUnavailable);

        if (shippingAddress is null)
            return Result<Order>.Failure(OrderErrors.ShippingAddressRequired);

        if (shippingAddress.UserId != userId)
            return Result<Order>.Failure(OrderErrors.ShippingAddressNotOwned);

        if (basketItems is null || basketItems.Count == 0)
            return Result<Order>.Failure(OrderErrors.EmptyBasket);

        var order = new Order
        {
            Id = id,
            UserId = userId,
            Status = OrderStatus.Pending,
            DeliveryMethodId = deliveryMethod.Id,
            DeliveryMethodName = deliveryMethod.Name,
            DeliveryMethodPrice = deliveryMethod.Price,
            DeliveryMethodEstimatedTime = deliveryMethod.EstimatedDeliveryTime,
            ShippingRecipientFirstName = shippingAddress.RecipientFirstName,
            ShippingRecipientLastName = shippingAddress.RecipientLastName,
            ShippingPhoneNumber = shippingAddress.PhoneNumber,
            ShippingCountry = shippingAddress.Country,
            ShippingCity = shippingAddress.City,
            ShippingStreet = shippingAddress.Street,
            ShippingPostalCode = shippingAddress.PostalCode,
            ShippingCost = deliveryMethod.Price,
            CreatedAt = DateTimeOffset.UtcNow
        };

        foreach (var line in basketItems)
        {
            var snapshotResult = ProductItemOrdered.Create(
                line.ProductId,
                line.ProductName,
                line.PictureUrl,
                line.UnitPrice);

            if (snapshotResult.IsFailure)
                return Result<Order>.Failure(snapshotResult.Error);

            var itemResult = OrderItem.Create(
                Guid.NewGuid(),
                snapshotResult.Value,
                line.Quantity);

            if (itemResult.IsFailure)
                return Result<Order>.Failure(itemResult.Error);

            var item = itemResult.Value;
            item.AssignOrder(id);
            order._items.Add(item);
        }

        order.SubTotal = order._items.Sum(i => i.LineTotal);
        order.Total = order.SubTotal + order.ShippingCost;

        return Result<Order>.Success(order);
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.CannotCancel);

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result AttachPaymentIntent(string paymentIntentId)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidPaymentState);

        if (string.IsNullOrWhiteSpace(paymentIntentId))
            return Result.Failure(OrderErrors.InvalidPaymentIntent);

        PaymentIntentId = paymentIntentId.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result MarkAsPaid(string paymentIntentId)
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.CannotPayCancelled);

        if (Status is not OrderStatus.Pending and not OrderStatus.Processing)
        {
            if (PaymentIntentId == paymentIntentId)
                return Result.Success();

            return Result.Failure(OrderErrors.InvalidPaymentState);
        }

        if (!string.IsNullOrWhiteSpace(PaymentIntentId)
            && PaymentIntentId != paymentIntentId)
            return Result.Failure(OrderErrors.PaymentIntentMismatch);

        PaymentIntentId = paymentIntentId;
        PaidAtUtc = DateTimeOffset.UtcNow;
        Status = OrderStatus.Processing;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }
}
