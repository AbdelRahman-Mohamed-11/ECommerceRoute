using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products.Queries;

public sealed class GetByIdProductQuery(IProductQueryService productQueryService)
{
    public async Task<Result<GetByIdProductResponse>> ExecuteAsync(Guid id)
    {
        var product = await productQueryService.GetByIdProductAsync(id);
        
        if (product == null)
        {
            return Result<GetByIdProductResponse>.Failure(ProductErrors.NotFound);
        }

        return Result<GetByIdProductResponse>.Success(product);
    }
}
