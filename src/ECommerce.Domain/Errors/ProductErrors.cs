using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "Product.NotFound",
            "Product was not found.");

    public static readonly Error InvalidId =
        Error.Validation(
            "Product.InvalidId",
            "Product id is required.");

    public static readonly Error InvalidName =
        Error.Validation(
            "Product.InvalidName",
            "Product name is required.");

    public static readonly Error NameTooLong =
        Error.Validation(
            "Product.NameTooLong",
            $"Product name cannot exceed {Entities.Product.MaxNameLength} characters.");

    public static readonly Error InvalidDescription =
        Error.Validation(
            "Product.InvalidDescription",
            "Product description is required.");

    public static readonly Error DescriptionTooLong =
        Error.Validation(
            "Product.DescriptionTooLong",
            $"Product description cannot exceed {Entities.Product.MaxDescriptionLength} characters.");

    public static readonly Error InvalidPictureUrl =
        Error.Validation(
            "Product.InvalidPictureUrl",
            "Product picture URL is required.");

    public static readonly Error PictureUrlTooLong =
        Error.Validation(
            "Product.PictureUrlTooLong",
            $"Product picture URL cannot exceed {Entities.Product.MaxPictureUrlLength} characters.");

    public static readonly Error InvalidPrice =
        Error.Validation(
            "Product.InvalidPrice",
            "Price must be greater than zero.");

    public static readonly Error InvalidBrand =
        Error.Validation(
            "Product.InvalidBrand",
            "Product brand is required.");

    public static readonly Error InvalidType =
        Error.Validation(
            "Product.InvalidType",
            "Product type is required.");

    public static readonly Error DuplicateName =
        Error.Conflict(
            "Product.DuplicateName",
            "Product name already exists.");
}
