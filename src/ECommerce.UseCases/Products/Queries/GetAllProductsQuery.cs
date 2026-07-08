using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries;

public sealed class GetAllProductsQuery(IReadRepository<Product> repository)
{
    public async Task<Result<IReadOnlyList<GetAllProductsResponse>>> ExecuteAsync(CancellationToken ct = default)
    {
        var products = await repository.ListAsync(new ProductsListSpecification(), ct);
        return Result<IReadOnlyList<GetAllProductsResponse>>.Success(products);
    }
}
