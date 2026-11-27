using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Renmei.Application.Abstractions;
using Renmei.Domain.Enums;
using Renmei.Domain.Users;

namespace Renmei.Application.Features.Auth;

public sealed class AuthService(
    IApplicationDbContext db,
    IPasswordHasher hasher,
    TimeProvider clock,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly Lazy<string> _dummyHash = new(() => hasher.Hash("dummy-password-not-used-for-timing"));

    public async Task<AuthResult<Guid>> RegisterAsync(RegisterCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);

        var email = NormalizeEmail(command.Email);
        if (!IsAcceptableEmail(email) || !IsAcceptablePassword(command.Password))
        {
            return new(default, AuthError.InvalidInput);
        }

        var displayName = string.IsNullOrWhiteSpace(command.DisplayName) ? null : command.DisplayName.Trim();
        if (displayName is { Length: > 100 })
        {
            return new(default, AuthError.InvalidInput);
        }

        if (await db.Users.AnyAsync(u => u.Email == email, ct))
        {
            return new(default, AuthError.EmailTaken);
        }

        var nowUtc = clock.GetUtcNow().UtcDateTime;
        var passwordHash = hasher.Hash(command.Password);

        var user = new User(email, passwordHash, displayName, nowUtc, emailConfirmed: false);
        user.AddRole(RoleIds.User);

        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return new(default, AuthError.EmailTaken);
        }

        logger.LogInformation("Registered new user account {UserId}.", user.Id);
        return new(user.Id);
    }

    public async Task<AuthResult<User>> AuthenticateAsync(string email, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password) || password.Length > 128)
        {
            hasher.Verify(_dummyHash.Value, "dummy-input");
            return new(default, AuthError.InvalidCredentials);
        }

        var normalizedEmail = NormalizeEmail(email);
        var user = await db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

        if (user is null)
        {
            hasher.Verify(_dummyHash.Value, password);
            return new(default, AuthError.InvalidCredentials);
        }

        var check = hasher.Verify(user.PasswordHash, password);
        if (check == PasswordCheck.Failed)
        {
            return new(default, AuthError.InvalidCredentials);
        }

        if (!user.CanSignIn)
        {
            logger.LogWarning("Sign-in blocked for inactive user account {UserId}.", user.Id);
            return new(default, AuthError.AccountLocked);
        }

        if (check == PasswordCheck.SuccessRehashNeeded)
        {
            user.ChangePasswordHash(hasher.Hash(password));
        }

        user.RecordLogin(clock.GetUtcNow().UtcDateTime);
        await db.SaveChangesAsync(ct);

        return new(user);
    }

    private static string NormalizeEmail(string? email)
        => string.IsNullOrWhiteSpace(email) ? string.Empty : email.Trim().ToLowerInvariant();

    private static bool IsAcceptableEmail(string email)
    {
        if (email.Length is < 3 or > 256 || email.Any(char.IsWhiteSpace))
        {
            return false;
        }

        var atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex != email.LastIndexOf('@') || atIndex == email.Length - 1)
        {
            return false;
        }

        var domain = email[(atIndex + 1)..];
        return domain.Contains('.') && !domain.StartsWith('.') && !domain.EndsWith('.');
    }

    private static bool IsAcceptablePassword(string? password)
        => password is not null && password.Length is >= 8 and <= 128;

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("2601", StringComparison.Ordinal) == true
           || ex.InnerException?.Message.Contains("2627", StringComparison.Ordinal) == true
           || ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true;
}