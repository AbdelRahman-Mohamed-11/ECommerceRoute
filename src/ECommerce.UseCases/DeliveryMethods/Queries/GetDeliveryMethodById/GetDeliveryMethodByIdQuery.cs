using ECommerce.Domain.Shared;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethodById;

public sealed record GetDeliveryMethodByIdQuery(Guid Id)
    : IQuery<Result<DeliveryMethodResponse>>;
