using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Email,
    string Code) : ICommand<Result>;
