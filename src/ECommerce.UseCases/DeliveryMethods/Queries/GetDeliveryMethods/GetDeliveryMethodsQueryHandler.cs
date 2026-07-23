using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethods;

public sealed class GetDeliveryMethodsQueryHandler(IReadRepository<DeliveryMethod> repository)
    : IRequestHandler<GetDeliveryMethodsQuery, Result<IReadOnlyList<DeliveryMethodResponse>>>
{
    public async Task<Result<IReadOnlyList<DeliveryMethodResponse>>> Handle(
        GetDeliveryMethodsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync(
            new DeliveryMethodsListSpecification(request.AvailableOnly),
            cancellationToken);

        return Result<IReadOnlyList<DeliveryMethodResponse>>.Success(items);
    }
}
