namespace ECommerce.UseCases.Common.Models;

public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAtUtc);
