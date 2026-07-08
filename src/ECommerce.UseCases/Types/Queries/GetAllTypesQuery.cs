using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Types.Dtos;
using ECommerce.UseCases.Types.Specifications;

namespace ECommerce.UseCases.Types.Queries;

public sealed class GetAllTypesQuery(IReadRepository<ProductType> repository)
{
    public async Task<Result<IReadOnlyList<GetAllTypesResponse>>> ExecuteAsync(CancellationToken ct = default)
    {
        var types = await repository.ListAsync(new TypesListSpecification(), ct);
        return Result<IReadOnlyList<GetAllTypesResponse>>.Success(types);
    }
}
