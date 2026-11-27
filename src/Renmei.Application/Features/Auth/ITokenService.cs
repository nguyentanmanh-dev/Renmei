using Renmei.Domain.Users;

namespace Renmei.Application.Features.Auth;

public interface ITokenService
{
    Task<TokenPairDto> IssueAsync(User user, CancellationToken ct);
}