using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class DeliveryMethodErrors
{
    public static readonly Error InvalidId =
        Error.Validation("DeliveryMethod.InvalidId", "Delivery method id is required.");

    public static readonly Error InvalidName =
        Error.Validation("DeliveryMethod.InvalidName", "Delivery method name is required.");

    public static readonly Error NameTooLong =
        Error.Validation(
            "DeliveryMethod.NameTooLong",
            $"Delivery method name cannot exceed {Entities.DeliveryMethod.MaxNameLength} characters.");

    public static readonly Error InvalidPrice =
        Error.Validation("DeliveryMethod.InvalidPrice", "Delivery price cannot be negative.");

    public static readonly Error InvalidDeliveryTime =
        Error.Validation("DeliveryMethod.InvalidDeliveryTime", "Estimated delivery time is required.");

    public static readonly Error NotFound =
        Error.NotFound("DeliveryMethod.NotFound", "Delivery method was not found.");

    public static readonly Error NameAlreadyExists =
        Error.Conflict("DeliveryMethod.NameAlreadyExists", "A delivery method with this name already exists.");
}
