using ECommerce.Domain.Entities;
using ECommerce.UseCases.Brands.Dtos;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Brands.Specifications;

public sealed class BrandsListSpecification : Specification<ProductBrand, GetAllBrandsResponse>
{
    public BrandsListSpecification()
    {
        Query
            .OrderBy(brand => brand.Name)
            .Select(brand => new GetAllBrandsResponse(brand.Id, brand.Name));
    }
}
