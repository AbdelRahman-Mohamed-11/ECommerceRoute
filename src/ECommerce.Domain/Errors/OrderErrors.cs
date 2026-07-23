using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class OrderErrors
{
    public static readonly Error InvalidId =
        Error.Validation("Order.InvalidId", "Order id is required.");

    public static readonly Error InvalidUserId =
        Error.Validation("Order.InvalidUserId", "User id is required.");

    public static readonly Error InvalidItemId =
        Error.Validation("Order.InvalidItemId", "Order item id is required.");

    public static readonly Error InvalidProductId =
        Error.Validation("Order.InvalidProductId", "Product id is required.");

    public static readonly Error InvalidProductName =
        Error.Validation("Order.InvalidProductName", "Product name is required.");

    public static readonly Error InvalidPictureUrl =
        Error.Validation("Order.InvalidPictureUrl", "Product picture URL is required.");

    public static readonly Error InvalidUnitPrice =
        Error.Validation("Order.InvalidUnitPrice", "Unit price cannot be negative.");

    public static readonly Error InvalidQuantity =
        Error.Validation("Order.InvalidQuantity", "Quantity must be at least 1.");

    public static readonly Error EmptyBasket =
        Error.Validation("Order.EmptyBasket", "Cannot create an order from an empty basket.");

    public static readonly Error NotFound =
        Error.NotFound("Order.NotFound", "Order was not found.");

    public static readonly Error Unauthorized =
        Error.Unauthorized("Order.Unauthorized", "You must be signed in to manage orders.");

    public static readonly Error Forbidden =
        Error.Forbidden("Order.Forbidden", "You do not have access to this order.");

    public static readonly Error CannotCancel =
        Error.Conflict("Order.CannotCancel", "Only pending orders can be cancelled.");

    public static readonly Error DeliveryMethodRequired =
        Error.Validation("Order.DeliveryMethodRequired", "Delivery method is required.");

    public static readonly Error DeliveryMethodUnavailable =
        Error.Validation("Order.DeliveryMethodUnavailable", "The selected delivery method is not available.");

    public static readonly Error ShippingAddressRequired =
        Error.Validation("Order.ShippingAddressRequired", "Shipping address is required.");


    public static readonly Error ShippingAddressNotFound =
        Error.NotFound("Order.ShippingAddressNotFound", "Shipping address was not found.");

    public static readonly Error ShippingAddressNotOwned =
        Error.Forbidden("Order.ShippingAddressNotOwned", "Shipping address does not belong to the current user.");
}
