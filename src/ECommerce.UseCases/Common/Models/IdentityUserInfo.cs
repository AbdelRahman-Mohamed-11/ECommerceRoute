namespace ECommerce.UseCases.Common.Models;

public sealed record IdentityUserInfo(
    Guid UserId,
    string Email,
    string? DisplayName);
