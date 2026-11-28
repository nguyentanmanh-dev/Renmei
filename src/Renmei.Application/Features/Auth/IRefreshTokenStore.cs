using Renmei.Domain.Users;

namespace Renmei.Application.Features.Auth;

public interface IRefreshTokenStore
{
    Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct);
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<bool> TryRevokeAsync(Guid tokenId, DateTime nowUtc, CancellationToken ct);
    Task<int> RevokeAllActiveAsync(Guid userId, DateTime nowUtc, CancellationToken ct);
}