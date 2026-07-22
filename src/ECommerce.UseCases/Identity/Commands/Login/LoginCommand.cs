using ECommerce.Domain.Shared;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<Result<AuthResponse>>;
