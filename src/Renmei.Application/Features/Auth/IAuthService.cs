using Renmei.Domain.Users;

namespace Renmei.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResult<Guid>> RegisterAsync(RegisterCommand command, CancellationToken ct);
    Task<AuthResult<User>> AuthenticateAsync(string email, string password, CancellationToken ct);
}