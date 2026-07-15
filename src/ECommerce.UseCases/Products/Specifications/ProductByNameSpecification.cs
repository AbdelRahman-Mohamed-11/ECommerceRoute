using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class ProductByNameSpecification : Specification<Product>
{
    public ProductByNameSpecification(string name) =>
        Query.Where(p => p.Name == name);
}
