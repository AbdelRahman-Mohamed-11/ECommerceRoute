using ECommerce.Domain.Shared;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<Result<UserProfileResponse>>;
