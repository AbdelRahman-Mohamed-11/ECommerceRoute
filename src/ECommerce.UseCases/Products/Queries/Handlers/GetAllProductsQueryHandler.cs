using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries.Handlers;

public sealed class GetAllProductsQueryHandler(IReadRepository<Product> repository)
    : IRequestHandler<GetAllProductsQuery, Result<IReadOnlyList<GetAllProductsResponse>>>
{
    public async Task<Result<IReadOnlyList<GetAllProductsResponse>>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await repository.ListAsync(new ProductsListSpecification(), cancellationToken);
        return Result<IReadOnlyList<GetAllProductsResponse>>.Success(products);
    }
}
