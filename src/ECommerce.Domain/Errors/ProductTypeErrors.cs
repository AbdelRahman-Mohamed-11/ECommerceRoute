using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class ProductTypeErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "ProductType.NotFound",
            "Type was not found.");

    public static readonly Error InvalidId =
        Error.Validation(
            "ProductType.InvalidId",
            "Type id is required.");

    public static readonly Error InvalidName =
        Error.Validation(
            "ProductType.InvalidName",
            "Type name is required.");
}
