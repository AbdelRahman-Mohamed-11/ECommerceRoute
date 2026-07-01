using ECommerce.Domain.Shared;
using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products.Queries;

public sealed class GetAllProductsQuery(IProductQueryService productQueryService)
{
    public async Task<Result<IReadOnlyList<GetAllProductsResponse>>> ExecuteAsync()
    {
        var products = await productQueryService.GetAllProductsAsync();
        return Result<IReadOnlyList<GetAllProductsResponse>>.Success(products);
    }
}
