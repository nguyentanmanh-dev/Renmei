using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renmei.Application.Abstractions;
using Renmei.Domain.Users;

namespace Renmei.Infrastructure.Seeding;

internal sealed class AdminSeeder(
    IApplicationDbContext db,
    Application.Abstractions.IPasswordHasher hasher,
    IClock clock,
    IOptions<AdminSeedOptions> options,
    ILogger<AdminSeeder> log) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        var o = options.Value;
        if (string.IsNullOrWhiteSpace(o.Email) || string.IsNullOrWhiteSpace(o.Password))
        {
            log.LogInformation("Admin seed skipped: no configuration provided.");
            return;
        }

        if (!await db.Roles.AnyAsync(r => r.Id == RoleIds.Admin, ct))
            throw new InvalidOperationException("Role 'Admin' is missing. Apply migrations before seeding.");

        if (await db.UserRoles.AnyAsync(x => x.RoleId == RoleIds.Admin, ct))
        {
            log.LogInformation("An admin already exists. Seed skipped.");
            return;
        }

        var email = o.Email.Trim().ToLowerInvariant();
        if (!IsValidEmail(email) || !IsAcceptablePassword(o.Password))
            throw new InvalidOperationException("Seed admin configuration is invalid.");

        if (await db.Users.AnyAsync(u => u.Email == email, ct))
        {
            log.LogWarning("The configured admin email is already used by a non-admin account. Seed skipped.");
            return;
        }

        var user = new User(
            email,
            hasher.Hash(o.Password),
            string.IsNullOrWhiteSpace(o.DisplayName) ? "Admin" : o.DisplayName.Trim(),
            clock.UtcNow.UtcDateTime,
            emailConfirmed: true);

        user.AddRole(RoleIds.Admin);
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        log.LogInformation("Initial admin account created.");
    }

    private static bool IsValidEmail(string e) => e.Length is > 3 and <= 256 && e.Contains('@') && !e.Contains(' ');
    private static bool IsAcceptablePassword(string p) => p.Length >= 12;
}