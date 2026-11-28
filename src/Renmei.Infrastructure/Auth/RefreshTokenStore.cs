using Microsoft.EntityFrameworkCore;
using Renmei.Application.Features.Auth;
using Renmei.Domain.Users;
using Renmei.Infrastructure.Persistence;

namespace Renmei.Infrastructure.Auth;

internal sealed class RefreshTokenStore(ApplicationDbContext db) : IRefreshTokenStore
{
    public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct)
        => db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> TryRevokeAsync(Guid tokenId, DateTime nowUtc, CancellationToken ct)
    {
        var affected = await db.RefreshTokens
            .Where(t => t.Id == tokenId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, nowUtc), ct);

        return affected == 1;
    }

    public Task<int> RevokeAllActiveAsync(Guid userId, DateTime nowUtc, CancellationToken ct)
        => db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, nowUtc), ct);
}