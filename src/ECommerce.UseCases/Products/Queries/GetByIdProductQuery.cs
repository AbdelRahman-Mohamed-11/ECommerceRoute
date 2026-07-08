using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries;

public sealed class GetByIdProductQuery(IReadRepository<Product> repository)
{
    public async Task<Result<GetByIdProductResponse>> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await repository.FirstOrDefaultAsync(new ProductByIdSpecification(id), ct);

        if (product is null)
            return Result<GetByIdProductResponse>.Failure(ProductErrors.NotFound);

        return product;
    }
}
