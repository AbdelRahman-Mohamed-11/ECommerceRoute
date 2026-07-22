namespace ECommerce.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public const int MaxTokenHashLength = 64;

    private RefreshToken()
    {
    }

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc is not null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public static RefreshToken Create(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (id == Guid.Empty)
            throw new ArgumentException("Id is required.", nameof(id));

        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));

        return new RefreshToken
        {
            Id = id,
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Revoke(string? replacedByTokenHash = null)
    {
        if (IsRevoked)
            return;

        RevokedAtUtc = DateTimeOffset.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
