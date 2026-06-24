using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public class ProductBrand : BaseEntity
{
    public string Name { get; private set; } = null!;

    public ICollection<Product> Products { get; private set; } = [];

    private ProductBrand()
    {
    }

    public static Result<ProductBrand> Create(Guid id, string name)
    {
        if (id == Guid.Empty)
            return Result<ProductBrand>.Failure(ProductBrandErrors.InvalidId);

        if (string.IsNullOrWhiteSpace(name))
            return Result<ProductBrand>.Failure(ProductBrandErrors.InvalidName);

        var productBrand = new ProductBrand
        {
            Id = id,
            Name = name.Trim()
        };

        return Result<ProductBrand>.Success(productBrand);
    }
}
