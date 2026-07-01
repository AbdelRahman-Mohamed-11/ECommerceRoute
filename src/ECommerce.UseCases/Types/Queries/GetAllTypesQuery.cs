using ECommerce.Domain.Shared;
using ECommerce.UseCases.Types.Dtos;

namespace ECommerce.UseCases.Types.Queries;

public sealed class GetAllTypesQuery(IProductTypeQueryService productTypeQueryService)
{
    public async Task<Result<IReadOnlyList<GetAllTypesResponse>>> ExecuteAsync(CancellationToken ct = default)
    {
        var types = await productTypeQueryService.GetAllTypesAsync(ct);
        return Result<IReadOnlyList<GetAllTypesResponse>>.Success(types);
    }
}
