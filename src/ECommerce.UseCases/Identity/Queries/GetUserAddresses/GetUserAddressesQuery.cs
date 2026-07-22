using ECommerce.Domain.Shared;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Queries.GetUserAddresses;

public sealed record GetUserAddressesQuery
    : IQuery<Result<IReadOnlyList<UserAddressResponse>>>;
