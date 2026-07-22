namespace ECommerce.UseCases.Common;

public record AccessTokenResult(string Token, DateTimeOffset ExpiresAtUtc);
