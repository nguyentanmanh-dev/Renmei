using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renmei.Application.Abstractions;
using Renmei.Domain.Users;
using Renmei.Infrastructure.Persistence;
using Renmei.Infrastructure.Security;
using Renmei.Infrastructure.Seeding;

namespace Renmei.Infrastructure.Tests.Seeding;

public sealed class AdminSeederTests
{
    private sealed class FakeClock(DateTimeOffset now) : IClock
    {
        public DateTimeOffset UtcNow => now;
    }

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Messages.Add(formatter(state, exception));
    }

    private static (AdminSeeder Seeder, ApplicationDbContext Db, ListLogger<AdminSeeder> Logger, IdentityPasswordHasher Hasher) Arrange(
        string? email = null,
        string? password = null,
        string? displayName = "Admin",
        bool seedRoles = true)
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new ApplicationDbContext(dbOptions);
        if (seedRoles)
        {
            db.Database.EnsureCreated();
        }

        var hasher = new IdentityPasswordHasher();
        var logger = new ListLogger<AdminSeeder>();
        var clock = new FakeClock(new DateTimeOffset(2026, 11, 30, 0, 0, 0, TimeSpan.Zero));
        var opts = Options.Create(new AdminSeedOptions
        {
            Email = email,
            Password = password,
            DisplayName = displayName
        });

        var seeder = new AdminSeeder(db, hasher, clock, opts, logger);
        return (seeder, db, logger, hasher);
    }

    [Fact]
    public async Task Creates_single_confirmed_admin_when_configured_and_no_admin_exists()
    {
        var (seeder, db, _, _) = Arrange(email: "Admin@Example.com", password: "a-very-long-secret-1");

        await seeder.SeedAsync();

        var user = await db.Users.Include(u => u.Roles).SingleAsync();
        Assert.Equal("admin@example.com", user.Email);
        Assert.True(user.EmailConfirmed);
        Assert.Single(user.Roles, r => r.RoleId == RoleIds.Admin);
    }

    [Fact]
    public async Task Does_not_create_second_admin_or_overwrite_password_on_rerun()
    {
        var (seeder, db, _, _) = Arrange(email: "admin@example.com", password: "a-very-long-secret-1");

        await seeder.SeedAsync();
        var firstHash = (await db.Users.SingleAsync()).PasswordHash;

        await seeder.SeedAsync();

        Assert.Equal(1, await db.Users.CountAsync());
        Assert.Equal(firstHash, (await db.Users.SingleAsync()).PasswordHash);
    }

    [Fact]
    public async Task Skips_silently_when_configuration_is_missing()
    {
        var (seeder, db, _, _) = Arrange(email: null, password: null);

        await seeder.SeedAsync();

        Assert.Empty(await db.Users.ToListAsync());
    }

    [Theory]
    [InlineData("invalid-email", "a-very-long-secret-1")]
    [InlineData("admin@example.com", "short123")]
    public async Task Throws_InvalidOperationException_when_email_or_password_is_invalid(string email, string password)
    {
        var (seeder, _, _, _) = Arrange(email: email, password: password);

        await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.SeedAsync());
    }

    [Fact]
    public async Task Skips_and_does_not_elevate_when_email_belongs_to_existing_non_admin_user()
    {
        var (seeder, db, _, hasher) = Arrange(email: "user@example.com", password: "a-very-long-secret-1");
        var existing = new User("user@example.com", hasher.Hash("other-password-123"), "Learner", DateTime.UtcNow, true);
        existing.AddRole(RoleIds.User);
        db.Users.Add(existing);
        await db.SaveChangesAsync();

        await seeder.SeedAsync();

        Assert.Equal(1, await db.Users.CountAsync());
        Assert.DoesNotContain(await db.UserRoles.ToListAsync(), ur => ur.RoleId == RoleIds.Admin);
    }

    [Fact]
    public async Task Throws_clear_exception_when_admin_role_is_missing()
    {
        var (seeder, _, _, _) = Arrange(email: "admin@example.com", password: "a-very-long-secret-1", seedRoles: false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.SeedAsync());
        Assert.Contains("Role 'Admin' is missing", ex.Message);
    }

    [Fact]
    public void PasswordHasher_hashes_and_verifies_correctly()
    {
        var hasher = new IdentityPasswordHasher();
        const string raw = "SuperSecretPassword#2026";

        var hash = hasher.Hash(raw);

        Assert.NotEqual(raw, hash);
        Assert.Equal(PasswordCheck.Success, hasher.Verify(hash, raw));
        Assert.Equal(PasswordCheck.Failed, hasher.Verify(hash, "WrongPassword#2026"));
    }

    [Fact]
    public async Task Logs_never_contain_plaintext_password()
    {
        const string secret = "MyUltraSecretPassword#99";
        var (seeder, _, logger, _) = Arrange(email: "admin@example.com", password: secret);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        Assert.All(logger.Messages, msg => Assert.DoesNotContain(secret, msg));
    }

    [Fact]
    public async Task EnsureCreated_seeds_three_roles_with_fixed_ids()
    {
        var (_, db, _, _) = Arrange(seedRoles: true);

        var roles = await db.Roles.OrderBy(r => r.Id).ToListAsync();

        Assert.Equal(3, roles.Count);
        Assert.Equal((RoleIds.User, RoleNames.User), (roles[0].Id, roles[0].Name));
        Assert.Equal((RoleIds.Editor, RoleNames.Editor), (roles[1].Id, roles[1].Name));
        Assert.Equal((RoleIds.Admin, RoleNames.Admin), (roles[2].Id, roles[2].Name));
    }
}