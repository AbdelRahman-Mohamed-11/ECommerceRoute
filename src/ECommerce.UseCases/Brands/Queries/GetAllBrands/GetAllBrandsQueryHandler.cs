using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Brands.Dtos;
using ECommerce.UseCases.Brands.Specifications;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Brands.Queries.GetAllBrands;

public sealed class GetAllBrandsQueryHandler(IReadRepository<ProductBrand> repository)
    : IRequestHandler<GetAllBrandsQuery, Result<IReadOnlyList<GetAllBrandsResponse>>>
{
    public async Task<Result<IReadOnlyList<GetAllBrandsResponse>>> Handle(
        GetAllBrandsQuery request,
        CancellationToken cancellationToken)
    {
        var brands = await repository.ListAsync(new BrandsListSpecification(), cancellationToken);
        return Result<IReadOnlyList<GetAllBrandsResponse>>.Success(brands);
    }
}