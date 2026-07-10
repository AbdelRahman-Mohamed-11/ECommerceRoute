using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

// ProductByIdSpec — Application concrete spec (one named query per use case).
// Responsibility: define the exact query shape for "get product by id" — filter, includes, no tracking.
public sealed class ProductByIdSpec : Specification<Product>
{
    public ProductByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id)
            .Include(p => p.ProductBrand)
            .Include(p => p.ProductType)
            .AsNoTracking();
}

// AllProductsCatalogSpec — Application concrete spec for catalog listing.
public sealed class AllProductsCatalogSpec : Specification<Product>
{
    public AllProductsCatalogSpec() =>
        Query.Include(p => p.ProductBrand)
            .Include(p => p.ProductType)
            .OrderBy(p => p.Name)
            .AsNoTracking();
}
