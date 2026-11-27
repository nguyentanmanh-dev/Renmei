using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Renmei.Application.Abstractions;

namespace Renmei.Infrastructure.Security;

public sealed class IdentityPasswordHasher(IOptions<PasswordHasherOptions>? options = null) : IPasswordHasher
{
    private readonly PasswordHasher<object> _inner = new(options ?? Options.Create(new PasswordHasherOptions()));

    public string Hash(string password) => _inner.HashPassword(null!, password);

    public PasswordCheck Verify(string hash, string password) =>
        _inner.VerifyHashedPassword(null!, hash, password) switch
        {
            PasswordVerificationResult.Success => PasswordCheck.Success,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordCheck.SuccessRehashNeeded,
            _ => PasswordCheck.Failed
        };
}