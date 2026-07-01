using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products;

public interface IProductQueryService
{
    Task<IReadOnlyList<GetAllProductsResponse>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<GetByIdProductResponse?> GetByIdProductAsync(Guid id, CancellationToken cancellationToken = default);
}