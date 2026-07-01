using ECommerce.UseCases.Types.Dtos;

namespace ECommerce.UseCases.Types;

public interface IProductTypeQueryService
{
    Task<IReadOnlyList<GetAllTypesResponse>> GetAllTypesAsync(CancellationToken cancellationToken = default);
}
