using ECommerce.Domain.Shared;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? DisplayName) : ICommand<Result<EmailSentResponse>>;
