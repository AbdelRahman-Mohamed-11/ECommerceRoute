using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries.Handlers;

public sealed class GetByIdProductQueryHandler(IReadRepository<Product> repository)
    : IRequestHandler<GetByIdProductQuery, Result<GetByIdProductResponse>>
{
    public async Task<Result<GetByIdProductResponse>> Handle(
        GetByIdProductQuery request,
        CancellationToken cancellationToken)
    {
        var product = await repository.FirstOrDefaultAsync(
            new ProductByIdSpecification(request.Id),
            cancellationToken);

        if (product is null)
            return Result<GetByIdProductResponse>.Failure(ProductErrors.NotFound);

        return product;
    }
}
