namespace ECommerce.Domain.Entities;

public class ProductBrand : BaseEntity
{
    public string Name { get; private set; } = null!;

    public ICollection<Product> Products { get; private set; } = [];


    private ProductBrand() { }


    public static ProductBrand Create(Guid id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (id == Guid.Empty)
            throw new ArgumentException("Brand is is required", nameof(id));

        return new() { Id = id, Name = name.Trim()};
    }
}
