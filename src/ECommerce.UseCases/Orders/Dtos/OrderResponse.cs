namespace ECommerce.UseCases.Orders.Dtos;

public sealed record ProductItemOrderedResponse(
    Guid ProductId,
    string ProductName,
    string PictureUrl,
    decimal UnitPrice);

public sealed record OrderItemResponse(
    Guid Id,
    ProductItemOrderedResponse ItemOrdered,
    int Quantity,
    decimal LineTotal);

public sealed record OrderShippingAddressResponse(
    string RecipientFirstName,
    string RecipientLastName,
    string PhoneNumber,
    string Country,
    string City,
    string Street,
    string PostalCode);

public sealed record OrderResponse(
    Guid Id,
    Guid UserId,
    string Status,
    Guid DeliveryMethodId,
    string DeliveryMethodName,
    decimal DeliveryMethodPrice,
    string DeliveryMethodEstimatedTime,
    OrderShippingAddressResponse ShippingAddress,
    decimal SubTotal,
    decimal ShippingCost,
    decimal Total,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemResponse> Items);
