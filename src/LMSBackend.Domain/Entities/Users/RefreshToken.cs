using LMSBackend.Domain.Exceptions;

namespace LMSBackend.Domain.Entities.Users;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public DateTimeOffset? RevokedAt { get; private set; }
    public User User { get; private set; } = null!;

    private RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Revoke()
    {
        if (RevokedAt.HasValue)
            throw new DomainValidationException("Refresh token is already revoked.");

        IsRevoked = true;
        RevokedAt = DateTimeOffset.UtcNow;
    }
}