using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renmei.Application.Abstractions;
using Renmei.Application.Features.Auth;
using Renmei.Application.Tests.Support;
using Renmei.Domain.Enums;
using Renmei.Domain.Users;
using Renmei.Infrastructure.Security;

namespace Renmei.Application.Tests.Features.Auth;

public sealed class AuthServiceTests
{
    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class SpyPasswordHasher(IPasswordHasher inner) : IPasswordHasher
    {
        public int VerifyCallCount { get; private set; }
        public string Hash(string password) => inner.Hash(password);

        public PasswordCheck Verify(string hash, string password)
        {
            VerifyCallCount++;
            return inner.Verify(hash, password);
        }
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Messages.Add(formatter(state, exception));
    }

    private static IPasswordHasher FastHasher(int iterations = 1_000)
        => new IdentityPasswordHasher(Options.Create(new PasswordHasherOptions { IterationCount = iterations }));

    private static readonly DateTimeOffset FixedTime = new(2026, 11, 27, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task RegisterAsync_creates_unconfirmed_lowercase_user_with_User_role_and_salted_hash()
    {
        var db = new TestDb();
        var logger = new CapturingLogger<AuthService>();

        await using (var ctx = db.NewContext())
        {
            var sut = new AuthService(ctx, FastHasher(), new FixedTimeProvider(FixedTime), logger);
            var res = await sut.RegisterAsync(new RegisterCommand("  HocVien@Renmei.VN ", "MatKhau123", "Minh Anh"), CancellationToken.None);

            Assert.True(res.IsSuccess);
        }

        await using var verify = db.NewContext();
        var user = await verify.Users.Include(u => u.Roles).SingleAsync();

        Assert.Equal("hocvien@renmei.vn", user.Email);
        Assert.False(user.EmailConfirmed);
        Assert.Equal("Minh Anh", user.DisplayName);
        Assert.NotEqual("MatKhau123", user.PasswordHash);
        Assert.Contains(user.Roles, r => r.RoleId == RoleIds.User);

        // Không ghi mật khẩu, mã băm hay email đầy đủ vào log
        Assert.All(logger.Messages, msg =>
        {
            Assert.DoesNotContain("MatKhau123", msg);
            Assert.DoesNotContain(user.PasswordHash, msg);
            Assert.DoesNotContain("hocvien@renmei.vn", msg, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task RegisterAsync_returns_EmailTaken_for_case_insensitive_duplicate_email()
    {
        var db = new TestDb();
        await using (var ctx1 = db.NewContext())
        {
            var sut1 = new AuthService(ctx1, FastHasher(), new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());
            var first = await sut1.RegisterAsync(new RegisterCommand("A@x.com", "MatKhau123", null), CancellationToken.None);
            Assert.True(first.IsSuccess);
        }

        await using var ctx2 = db.NewContext();
        var sut2 = new AuthService(ctx2, FastHasher(), new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());
        var second = await sut2.RegisterAsync(new RegisterCommand("a@x.com", "MatKhauKhac123", null), CancellationToken.None);

        Assert.Equal(AuthError.EmailTaken, second.Error);
    }

    [Theory]
    [Trait("Requirement", "FR-001")]
    [InlineData("user@renmei.vn", "1234567")]
    [InlineData("user@renmei.vn", "123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789")]
    [InlineData("", "MatKhau123")]
    [InlineData("khong-co-a-cong.vn", "MatKhau123")]
    public async Task RegisterAsync_returns_InvalidInput_for_invalid_email_or_password_length(string email, string password)
    {
        var db = new TestDb();
        await using var ctx = db.NewContext();
        var sut = new AuthService(ctx, FastHasher(), new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());

        var res = await sut.RegisterAsync(new RegisterCommand(email, password, null), CancellationToken.None);

        Assert.Equal(AuthError.InvalidInput, res.Error);
    }

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task AuthenticateAsync_updates_LastLoginAt_on_valid_credentials()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            var sut = new AuthService(seed, FastHasher(), new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());
            await sut.RegisterAsync(new RegisterCommand("user@renmei.vn", "MatKhau123", null), CancellationToken.None);
        }

        var loginTime = FixedTime.AddHours(2);
        await using (var ctx = db.NewContext())
        {
            var sut = new AuthService(ctx, FastHasher(), new FixedTimeProvider(loginTime), new CapturingLogger<AuthService>());
            var res = await sut.AuthenticateAsync("USER@renmei.vn", "MatKhau123", CancellationToken.None);

            Assert.True(res.IsSuccess);
            Assert.Equal(loginTime.UtcDateTime, res.Value!.LastLoginAt);
        }

        await using var check = db.NewContext();
        var saved = await check.Users.AsNoTracking().SingleAsync();
        Assert.Equal(loginTime.UtcDateTime, saved.LastLoginAt);
    }

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task AuthenticateAsync_returns_same_InvalidCredentials_and_runs_dummy_verify_for_unknown_email()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            var sut = new AuthService(seed, FastHasher(), new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());
            await sut.RegisterAsync(new RegisterCommand("user@renmei.vn", "MatKhau123", null), CancellationToken.None);
        }

        var spy = new SpyPasswordHasher(FastHasher());
        await using var ctx = db.NewContext();
        var auth = new AuthService(ctx, spy, new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());

        var unknownEmailRes = await auth.AuthenticateAsync("khongtontai@renmei.vn", "MatKhau123", CancellationToken.None);
        Assert.Equal(1, spy.VerifyCallCount);

        var wrongPwdRes = await auth.AuthenticateAsync("user@renmei.vn", "SaiMatKhau", CancellationToken.None);
        Assert.Equal(2, spy.VerifyCallCount);

        Assert.Equal(AuthError.InvalidCredentials, unknownEmailRes.Error);
        Assert.Equal(wrongPwdRes.Error, unknownEmailRes.Error);
    }

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task AuthenticateAsync_reveals_AccountLocked_only_when_password_matches()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            var u = new User("locked@renmei.vn", FastHasher().Hash("MatKhau123"), null, FixedTime.UtcDateTime, false);
            typeof(User).GetProperty(nameof(User.Status))!.SetValue(u, UserStatus.Locked);
            seed.Users.Add(u);
            await seed.SaveChangesAsync();
        }

        await using var ctx = db.NewContext();
        var sut = new AuthService(ctx, FastHasher(), new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());

        // Mật khẩu sai -> phải trả InvalidCredentials (401), không được lộ AccountLocked (403)
        var wrongPwd = await sut.AuthenticateAsync("locked@renmei.vn", "SaiMatKhau", CancellationToken.None);
        Assert.Equal(AuthError.InvalidCredentials, wrongPwd.Error);

        // Mật khẩu đúng -> trả AccountLocked (403)
        var rightPwd = await sut.AuthenticateAsync("locked@renmei.vn", "MatKhau123", CancellationToken.None);
        Assert.Equal(AuthError.AccountLocked, rightPwd.Error);
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public async Task AuthenticateAsync_rehashes_legacy_password_hash_upon_successful_login()
    {
        var db = new TestDb();
        string oldHash;
        await using (var seed = db.NewContext())
        {
            oldHash = FastHasher(iterations: 1_000).Hash("MatKhau123");
            seed.Users.Add(new User("upgrade@renmei.vn", oldHash, null, FixedTime.UtcDateTime, false));
            await seed.SaveChangesAsync();
        }

        var upgradedHasher = FastHasher(iterations: 50_000);
        await using (var ctx = db.NewContext())
        {
            var sut = new AuthService(ctx, upgradedHasher, new FixedTimeProvider(FixedTime), new CapturingLogger<AuthService>());
            var res = await sut.AuthenticateAsync("upgrade@renmei.vn", "MatKhau123", CancellationToken.None);
            Assert.True(res.IsSuccess);
        }

        await using var verify = db.NewContext();
        var updatedUser = await verify.Users.AsNoTracking().SingleAsync();
        Assert.NotEqual(oldHash, updatedUser.PasswordHash);
        Assert.Equal(PasswordCheck.Success, upgradedHasher.Verify(updatedUser.PasswordHash, "MatKhau123"));
    }
}