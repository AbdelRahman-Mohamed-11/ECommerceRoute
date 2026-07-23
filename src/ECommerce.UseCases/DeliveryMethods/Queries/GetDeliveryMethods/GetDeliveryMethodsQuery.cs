using ECommerce.Domain.Shared;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethods;

public sealed record GetDeliveryMethodsQuery(
    bool AvailableOnly = true) : IQuery<Result<IReadOnlyList<DeliveryMethodResponse>>>;
