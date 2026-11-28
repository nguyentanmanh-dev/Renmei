using Renmei.Domain.Users;

namespace Renmei.Application.Features.Auth;

public interface ITokenService
{
    Task<AuthResponseDto> IssueAsync(User user, CancellationToken ct);
    Task<AuthResult<AuthResponseDto>> RefreshAsync(string rawToken, CancellationToken ct);
    Task RevokeAsync(string rawToken, CancellationToken ct);
}