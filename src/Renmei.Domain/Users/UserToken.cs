using Renmei.Domain.Enums;

namespace Renmei.Domain.Users;

public class UserToken
{
    private UserToken() { }

    public UserToken(Guid userId, TokenPurpose purpose, string tokenHash, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(tokenHash)) throw new ArgumentException("TokenHash is required.", nameof(tokenHash));

        Id = Guid.NewGuid();
        UserId = userId;
        Purpose = purpose;
        TokenHash = tokenHash;
        ExpiresAt = expiresAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public TokenPurpose Purpose { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
}