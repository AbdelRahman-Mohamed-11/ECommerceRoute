using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public sealed class DeliveryMethod : BaseEntity
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxDeliveryTimeLength = 100;

    private DeliveryMethod()
    {
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string EstimatedDeliveryTime { get; private set; } = null!;
    public bool IsAvailable { get; private set; }
    public int DisplayOrder { get; private set; }

    public static Result<DeliveryMethod> Create(
        Guid id,
        string name,
        decimal price,
        string estimatedDeliveryTime,
        string? description = null,
        bool isAvailable = true,
        int displayOrder = 0)
    {
        if (id == Guid.Empty)
            return Result<DeliveryMethod>.Failure(DeliveryMethodErrors.InvalidId);

        if (string.IsNullOrWhiteSpace(name))
            return Result<DeliveryMethod>.Failure(DeliveryMethodErrors.InvalidName);

        if (name.Trim().Length > MaxNameLength)
            return Result<DeliveryMethod>.Failure(DeliveryMethodErrors.NameTooLong);

        if (price < 0)
            return Result<DeliveryMethod>.Failure(DeliveryMethodErrors.InvalidPrice);

        if (string.IsNullOrWhiteSpace(estimatedDeliveryTime))
            return Result<DeliveryMethod>.Failure(DeliveryMethodErrors.InvalidDeliveryTime);

        return Result<DeliveryMethod>.Success(new DeliveryMethod
        {
            Id = id,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Price = price,
            EstimatedDeliveryTime = estimatedDeliveryTime.Trim(),
            IsAvailable = isAvailable,
            DisplayOrder = displayOrder,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    public Result Update(
        string name,
        decimal price,
        string estimatedDeliveryTime,
        string? description,
        bool isAvailable,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(DeliveryMethodErrors.InvalidName);

        if (name.Trim().Length > MaxNameLength)
            return Result.Failure(DeliveryMethodErrors.NameTooLong);

        if (price < 0)
            return Result.Failure(DeliveryMethodErrors.InvalidPrice);

        if (string.IsNullOrWhiteSpace(estimatedDeliveryTime))
            return Result.Failure(DeliveryMethodErrors.InvalidDeliveryTime);

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Price = price;
        EstimatedDeliveryTime = estimatedDeliveryTime.Trim();
        IsAvailable = isAvailable;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}
