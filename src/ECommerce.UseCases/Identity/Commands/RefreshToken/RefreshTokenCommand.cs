using ECommerce.Domain.Shared;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken) : ICommand<Result<AuthResponse>>;
