using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Types.Dtos;
using ECommerce.UseCases.Types.Specifications;

namespace ECommerce.UseCases.Types.Queries.GetAllTypes;

public sealed class GetAllTypesQueryHandler(IReadRepository<ProductType> repository)
    : IRequestHandler<GetAllTypesQuery, Result<IReadOnlyList<GetAllTypesResponse>>>
{
    public async Task<Result<IReadOnlyList<GetAllTypesResponse>>> Handle(
        GetAllTypesQuery request,
        CancellationToken cancellationToken)
    {
        var types = await repository.ListAsync(new TypesListSpecification(), cancellationToken);
        return Result<IReadOnlyList<GetAllTypesResponse>>.Success(types);
    }
}