using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Types.Dtos;

namespace ECommerce.UseCases.Types.Queries.GetAllTypes;

public sealed record GetAllTypesQuery : IQuery<Result<IReadOnlyList<GetAllTypesResponse>>>;