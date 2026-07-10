using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Brands.Dtos;
using ECommerce.UseCases.Brands.Specifications;

namespace ECommerce.UseCases.Brands.Queries;

public sealed class GetAllBrandsQuery(IReadRepository<ProductBrand> repository)
{
    public async Task<Result<IReadOnlyList<GetAllBrandsResponse>>> ExecuteAsync(CancellationToken ct = default)
    {
        var brands = await repository.ListAsync(new BrandsListSpecification(), ct);
        return Result<IReadOnlyList<GetAllBrandsResponse>>.Success(brands);
    }
}
