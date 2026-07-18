namespace ECommerce.UseCases.Identity.Dtos;

public sealed record UserProfileResponse(
    Guid UserId,
    string Email,
    string? DisplayName);
