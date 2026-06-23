namespace ECommerce.Domain.Entities;

public class ProductType : BaseEntity
{
    public string Name { get; private set; } = null!;

    public ICollection<Product> Products { get; private set; } = [];
}