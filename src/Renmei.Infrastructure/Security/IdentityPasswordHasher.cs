using Microsoft.AspNetCore.Identity;
using Renmei.Application.Abstractions;
using Renmei.Domain.Users;

namespace Renmei.Infrastructure.Security;

internal sealed class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _inner = new();

    public string Hash(string password) => _inner.HashPassword(null!, password);

    public bool Verify(string hash, string password)
        => _inner.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}