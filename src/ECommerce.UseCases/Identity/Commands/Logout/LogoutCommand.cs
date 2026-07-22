using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : ICommand<Result>;
