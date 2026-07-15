using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using System.Text.Json.Serialization;

namespace ECommerce.Domain.Entities;

public class BasketItem
{
    public const int MinQuantity = 1;
    public const int MaxQuantity = 99;


    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public string PictureUrl { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    [JsonConstructor]
    private BasketItem(Guid productId, string productName, string pictureUrl, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static Result<BasketItem> Create(Guid productId, string productName, 
        string pictureUrl, decimal unitPrice, int quantity)
    {
        if (productId == Guid.Empty)
            return Result<BasketItem>.Failure(BasketErrors.InvalidProductId);

        if (string.IsNullOrWhiteSpace(productName))
            return Result<BasketItem>.Failure(BasketErrors.InvalidProductName);

        if (string.IsNullOrWhiteSpace(pictureUrl))
            return Result<BasketItem>.Failure(BasketErrors.InvalidPictureUrl);

        if (unitPrice <= 0)
            return Result<BasketItem>.Failure(BasketErrors.InvalidUnitPrice);

        if (quantity is < MinQuantity or > MaxQuantity)
            return Result<BasketItem>.Failure(BasketErrors.InvalidQuantity);

        return Result<BasketItem>.Success(
            new BasketItem(
                productId,
                productName.Trim(),
                pictureUrl.Trim(),
                unitPrice,
                quantity));
    }

    public decimal LineTotal => UnitPrice * Quantity;


    public Result IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            return Result.Failure(BasketErrors.InvalidQuantityIncrement);

        var newQuantity = Quantity + amount;

        if (newQuantity > MaxQuantity)
            return Result.Failure(BasketErrors.QuantityTooHigh);

        Quantity = newQuantity;

        return Result.Success();
    }


    public Result SetQuantity(int quantity)
    {
        if (quantity is < MinQuantity or > MaxQuantity)
            return Result.Failure(BasketErrors.InvalidQuantity);

        Quantity = quantity;

        return Result.Success();
    }

    public Result UpdateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            return Result.Failure(BasketErrors.InvalidUnitPrice);

        UnitPrice = unitPrice;

        return Result.Success();
    }
}
