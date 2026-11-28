namespace Renmei.Application.Features.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
}

public sealed class TokenOptions
{
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 30;
    public int ReuseGraceSeconds { get; init; } = 10;
}

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc, int ExpiresInSeconds);

public interface IAccessTokenGenerator
{
    AccessTokenResult Generate(Guid userId, IReadOnlyCollection<string> roles);
}