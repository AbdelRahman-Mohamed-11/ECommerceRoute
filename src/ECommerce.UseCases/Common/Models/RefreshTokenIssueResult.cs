namespace ECommerce.UseCases.Common.Models;

public sealed record RefreshTokenIssueResult(
    Guid UserId,
    string Token,
    DateTimeOffset ExpiresAtUtc);
