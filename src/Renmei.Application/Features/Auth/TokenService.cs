using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renmei.Application.Abstractions;
using Renmei.Domain.Users;

namespace Renmei.Application.Features.Auth;

public sealed class TokenService(
    IApplicationDbContext db,
    IRefreshTokenStore store,
    IAccessTokenGenerator accessTokens,
    IOptions<TokenOptions> tokenOptions,
    TimeProvider clock,
    ILogger<TokenService> logger) : ITokenService
{
    public async Task<AuthResponseDto> IssueAsync(User user, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(user);

        var roleNames = await ResolveUserRoleNamesAsync(user, ct);
        var access = accessTokens.Generate(user.Id, roleNames);

        var (rawRefresh, refreshHash) = RefreshTokenSecrets.Generate();
        var refreshExpires = clock.GetUtcNow().UtcDateTime.AddDays(tokenOptions.Value.RefreshTokenDays);

        var entity = new RefreshToken(user.Id, refreshHash, refreshExpires);
        await store.AddAsync(entity, ct);

        var profile = new UserProfileDto(user.Id, user.Email, user.DisplayName, roleNames);
        return new AuthResponseDto(access.Token, rawRefresh, access.ExpiresInSeconds, profile);
    }

    public async Task<AuthResult<AuthResponseDto>> RefreshAsync(string rawToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return Invalid();
        }

        var now = clock.GetUtcNow().UtcDateTime;
        var rec = await store.FindByHashAsync(RefreshTokenSecrets.Hash(rawToken.Trim()), ct);
        if (rec is null || rec.ExpiresAt <= now)
        {
            return Invalid();
        }

        if (rec.RevokedAt is { } revokedAt)
        {
            if (now - revokedAt <= TimeSpan.FromSeconds(tokenOptions.Value.ReuseGraceSeconds))
            {
                return Invalid();
            }

            await store.RevokeAllActiveAsync(rec.UserId, now, ct);
            logger.LogWarning("Refresh token reuse detected for user {UserId}.", rec.UserId);
            return Invalid();
        }

        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == rec.UserId, ct);

        if (user is null || !user.CanSignIn)
        {
            await store.RevokeAllActiveAsync(rec.UserId, now, ct);
            return Invalid();
        }

        if (!await store.TryRevokeAsync(rec.Id, now, ct))
        {
            return Invalid();
        }

        return new(await IssueAsync(user, ct));
    }

    public async Task RevokeAsync(string rawToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return;
        }

        var rec = await store.FindByHashAsync(RefreshTokenSecrets.Hash(rawToken.Trim()), ct);
        if (rec is null || rec.RevokedAt is not null)
        {
            return;
        }

        await store.TryRevokeAsync(rec.Id, clock.GetUtcNow().UtcDateTime, ct);
    }

    private async Task<IReadOnlyList<string>> ResolveUserRoleNamesAsync(User user, CancellationToken ct)
    {
        var roleIds = user.Roles.Select(r => r.RoleId).Distinct().ToList();
        if (roleIds.Count == 0)
        {
            return [];
        }

        return await db.Roles
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id))
            .OrderBy(r => r.Id)
            .Select(r => r.Name)
            .ToListAsync(ct);
    }

    private static AuthResult<AuthResponseDto> Invalid()
        => new(default, AuthError.InvalidRefreshToken);
}