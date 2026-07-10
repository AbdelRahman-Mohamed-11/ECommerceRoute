using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Brands.Specifications;

// ProductBrandWithProductsSpec — Include + ThenInclude example.
// Responsibility: load a brand with its products and each product's type — no tracking.
public sealed class ProductBrandWithProductsSpec : Specification<ProductBrand>
{
    public ProductBrandWithProductsSpec(Guid brandId) =>
        Query.Where(b => b.Id == brandId)
            .Include(b => b.Products)
                .ThenInclude(p => p.ProductType)
            .AsNoTracking();
}
