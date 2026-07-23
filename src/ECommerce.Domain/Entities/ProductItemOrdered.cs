using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Immutable product snapshot captured at order time (name/price/image may change later on Product).
/// </summary>
public sealed class ProductItemOrdered
{
    public const int MaxProductNameLength = 200;
    public const int MaxPictureUrlLength = 500;

    private ProductItemOrdered()
    {
    }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public string PictureUrl { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }

    public static Result<ProductItemOrdered> Create(
        Guid productId,
        string productName,
        string pictureUrl,
        decimal unitPrice)
    {
        if (productId == Guid.Empty)
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidProductId);

        if (string.IsNullOrWhiteSpace(productName))
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidProductName);

        if (string.IsNullOrWhiteSpace(pictureUrl))
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidPictureUrl);

        if (unitPrice < 0)
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidUnitPrice);

        return Result<ProductItemOrdered>.Success(new ProductItemOrdered
        {
            ProductId = productId,
            ProductName = productName.Trim(),
            PictureUrl = pictureUrl.Trim(),
            UnitPrice = unitPrice
        });
    }
}
