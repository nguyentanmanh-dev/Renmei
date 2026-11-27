using Renmei.Application.Features.Auth;
using Renmei.Domain.Users;

namespace Renmei.WebApi.Services;

public sealed class PlaceholderTokenService : ITokenService
{
    public Task<TokenPairDto> IssueAsync(User user, CancellationToken ct)
        => throw new NotImplementedException("JWT issuance will be implemented in Week 7.");
}