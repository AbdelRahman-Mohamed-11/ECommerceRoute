using ECommerce.Domain.Shared;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(string? DisplayName)
    : ICommand<Result<UserProfileResponse>>;
