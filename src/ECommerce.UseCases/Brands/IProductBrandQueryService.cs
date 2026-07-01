using ECommerce.UseCases.Brands.Dtos;

namespace ECommerce.UseCases.Brands;

public interface IProductBrandQueryService
{
    Task<IReadOnlyList<GetAllBrandsResponse>> GetAllBrandsAsync(CancellationToken cancellationToken = default);
}
