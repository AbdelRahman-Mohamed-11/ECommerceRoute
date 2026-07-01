using ECommerce.Domain.Shared;
using ECommerce.UseCases.Brands.Dtos;

namespace ECommerce.UseCases.Brands.Queries;

public sealed class GetAllBrandsQuery(IProductBrandQueryService productBrandQueryService)
{
    public async Task<Result<IReadOnlyList<GetAllBrandsResponse>>> ExecuteAsync(CancellationToken ct = default)
    {
        var brands = await productBrandQueryService.GetAllBrandsAsync(ct);
        return Result<IReadOnlyList<GetAllBrandsResponse>>.Success(brands);
    }
}
