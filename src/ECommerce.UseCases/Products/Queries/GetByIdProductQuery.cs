using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products.Queries;

public sealed class GetByIdProductQuery(IProductQueryService productQueryService)
{
    public async Task<Result<GetByIdProductResponse>> ExecuteAsync(Guid id , CancellationToken ct = default)
    {
        var product = await productQueryService.GetByIdProductAsync(id, ct);
        
        if (product == null)
        {
            return Result<GetByIdProductResponse>.Failure(ProductErrors.NotFound);
        }

        return product;
    }
}
