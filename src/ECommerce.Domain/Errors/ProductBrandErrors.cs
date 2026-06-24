using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class ProductBrandErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "ProductBrand.NotFound",
            "Brand was not found.");

    public static readonly Error InvalidId =
        Error.Validation(
            "ProductBrand.InvalidId",
            "Brand id is required.");

    public static readonly Error InvalidName =
        Error.Validation(
            "ProductBrand.InvalidName",
            "Brand name is required.");
}
