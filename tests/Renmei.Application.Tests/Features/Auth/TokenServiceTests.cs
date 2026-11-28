using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renmei.Application.Features.Auth;
using Renmei.Application.Tests.Support;
using Renmei.Domain.Enums;
using Renmei.Domain.Users;
using Renmei.Infrastructure.Auth;
using Renmei.Infrastructure.Persistence;

namespace Renmei.Application.Tests.Features.Auth;

public sealed class TokenServiceTests
{
    private sealed class MutableTimeProvider(DateTimeOffset current) : TimeProvider
    {
        public DateTimeOffset Current { get; set; } = current;
        public override DateTimeOffset GetUtcNow() => Current;
    }

    private sealed class ThreadSafeFakeRefreshTokenStore : IRefreshTokenStore
    {
        private readonly object _gate = new();
        public List<RefreshToken> Tokens { get; } = [];

        public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct)
        {
            lock (_gate)
            {
                return Task.FromResult(Tokens.FirstOrDefault(t => t.TokenHash == tokenHash));
            }
        }

        public Task AddAsync(RefreshToken token, CancellationToken ct)
        {
            lock (_gate)
            {
                if (token.Id == Guid.Empty)
                {
                    typeof(RefreshToken).GetProperty(nameof(RefreshToken.Id))!.SetValue(token, Guid.NewGuid());
                }

                Tokens.Add(token);
                return Task.CompletedTask;
            }
        }

        public Task<bool> TryRevokeAsync(Guid tokenId, DateTime nowUtc, CancellationToken ct)
        {
            lock (_gate)
            {
                var found = Tokens.FirstOrDefault(t => t.Id == tokenId && t.RevokedAt == null);
                if (found is null)
                {
                    return Task.FromResult(false);
                }

                typeof(RefreshToken).GetProperty(nameof(RefreshToken.RevokedAt))!.SetValue(found, nowUtc);
                return Task.FromResult(true);
            }
        }

        public Task<int> RevokeAllActiveAsync(Guid userId, DateTime nowUtc, CancellationToken ct)
        {
            lock (_gate)
            {
                var active = Tokens.Where(t => t.UserId == userId && t.RevokedAt == null).ToList();
                foreach (var t in active)
                {
                    typeof(RefreshToken).GetProperty(nameof(RefreshToken.RevokedAt))!.SetValue(t, nowUtc);
                }

                return Task.FromResult(active.Count);
            }
        }
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Messages.Add(formatter(state, exception));
    }

    private static readonly string SigningKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

    private static (TokenService Sut, ThreadSafeFakeRefreshTokenStore Store, MutableTimeProvider Clock, CapturingLogger<TokenService> Logger)
        CreateSut(ApplicationDbContext db, DateTimeOffset start, ThreadSafeFakeRefreshTokenStore? sharedStore = null)
    {
        var store = sharedStore ?? new ThreadSafeFakeRefreshTokenStore();
        var clock = new MutableTimeProvider(start);
        var logger = new CapturingLogger<TokenService>();
        var jwtOpts = Options.Create(new JwtOptions { Issuer = "renmei-api", Audience = "renmei-clients", SigningKey = SigningKey });
        var tokOpts = Options.Create(new TokenOptions { AccessTokenMinutes = 15, RefreshTokenDays = 30, ReuseGraceSeconds = 10 });
        var generator = new JwtAccessTokenGenerator(jwtOpts, tokOpts, clock);

        return (new TokenService(db, store, generator, tokOpts, clock, logger), store, clock, logger);
    }

    private static async Task<User> SeedUserAsync(TestDb db, string email = "learner@renmei.vn", UserStatus status = UserStatus.Active)
    {
        await using var seed = db.NewContext();
        await seed.Database.EnsureCreatedAsync();

        if (!await seed.Roles.AnyAsync(r => r.Id == RoleIds.User))
        {
            var role = (Role)Activator.CreateInstance(typeof(Role), nonPublic: true)!;
            typeof(Role).GetProperty(nameof(Role.Id))!.SetValue(role, RoleIds.User);
            typeof(Role).GetProperty(nameof(Role.Name))!.SetValue(role, "User");
            seed.Roles.Add(role);
        }

        var user = new User(email, "hashed-pwd", "Học Viên", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), false);
        user.AddRole(RoleIds.User);
        typeof(User).GetProperty(nameof(User.Status))!.SetValue(user, status);
        seed.Users.Add(user);
        await seed.SaveChangesAsync();
        return user;
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public async Task IssueAsync_stores_only_sha256_hash_never_raw_token()
    {
        var db = new TestDb();
        var user = await SeedUserAsync(db);

        await using var ctx = db.NewContext();
        var (sut, store, _, _) = CreateSut(ctx, new DateTimeOffset(2026, 11, 28, 10, 0, 0, TimeSpan.Zero));

        var issued = await sut.IssueAsync(user, CancellationToken.None);

        var stored = Assert.Single(store.Tokens);
        Assert.NotEqual(issued.RefreshToken, stored.TokenHash);
        Assert.Equal(RefreshTokenSecrets.Hash(issued.RefreshToken), stored.TokenHash);
    }

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task RefreshAsync_rotates_token_and_rejects_expired_or_missing_tokens()
    {
        var db = new TestDb();
        var user = await SeedUserAsync(db);
        var t0 = new DateTimeOffset(2026, 11, 28, 10, 0, 0, TimeSpan.Zero);

        await using var ctx = db.NewContext();
        var (sut, store, clock, _) = CreateSut(ctx, t0);

        var first = await sut.IssueAsync(user, CancellationToken.None);

        clock.Current = t0.AddMinutes(5);
        var second = await sut.RefreshAsync(first.RefreshToken, CancellationToken.None);

        Assert.True(second.IsSuccess);
        Assert.NotEqual(first.RefreshToken, second.Value!.RefreshToken);
        Assert.NotNull(store.Tokens[0].RevokedAt);
        Assert.Null(store.Tokens[1].RevokedAt);

        Assert.Equal(AuthError.InvalidRefreshToken, (await sut.RefreshAsync("", CancellationToken.None)).Error);
        Assert.Equal(AuthError.InvalidRefreshToken, (await sut.RefreshAsync("non-existent-token", CancellationToken.None)).Error);

        // Quá hạn 30 ngày
        clock.Current = t0.AddDays(31);
        Assert.Equal(AuthError.InvalidRefreshToken, (await sut.RefreshAsync(second.Value.RefreshToken, CancellationToken.None)).Error);
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public async Task RefreshAsync_handles_grace_period_vs_reuse_theft_detection()
    {
        var db = new TestDb();
        var user = await SeedUserAsync(db);
        var t0 = new DateTimeOffset(2026, 11, 28, 10, 0, 0, TimeSpan.Zero);

        await using var ctx = db.NewContext();
        var (sut, store, clock, logger) = CreateSut(ctx, t0);

        var initial = await sut.IssueAsync(user, CancellationToken.None);
        var rotated = await sut.RefreshAsync(initial.RefreshToken, CancellationToken.None);
        Assert.True(rotated.IsSuccess);

        // Dùng lại trong ân hạn (5s <= 10s): trả 401 nhưng giữ nguyên token mới
        clock.Current = t0.AddSeconds(5);
        var withinGrace = await sut.RefreshAsync(initial.RefreshToken, CancellationToken.None);
        Assert.Equal(AuthError.InvalidRefreshToken, withinGrace.Error);
        Assert.Null(store.Tokens[1].RevokedAt);

        // Dùng lại sau ân hạn (15s > 10s): thu hồi toàn bộ token đang hoạt động của user
        clock.Current = t0.AddSeconds(15);
        var afterGrace = await sut.RefreshAsync(initial.RefreshToken, CancellationToken.None);
        Assert.Equal(AuthError.InvalidRefreshToken, afterGrace.Error);
        Assert.All(store.Tokens, t => Assert.NotNull(t.RevokedAt));

        // Log không chứa token thô, bản băm hay email
        Assert.All(logger.Messages, msg =>
        {
            Assert.DoesNotContain(initial.RefreshToken, msg);
            Assert.DoesNotContain(store.Tokens[0].TokenHash, msg);
            Assert.DoesNotContain("learner@renmei.vn", msg, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public async Task RefreshAsync_concurrent_calls_with_same_token_allow_only_one_winner()
    {
        var db = new TestDb();
        var user = await SeedUserAsync(db);
        var t0 = new DateTimeOffset(2026, 11, 28, 10, 0, 0, TimeSpan.Zero);
        var sharedStore = new ThreadSafeFakeRefreshTokenStore();

        string rawRefresh;
        await using (var issueCtx = db.NewContext())
        {
            var (issueSut, _, _, _) = CreateSut(issueCtx, t0, sharedStore);
            rawRefresh = (await issueSut.IssueAsync(user, CancellationToken.None)).RefreshToken;
        }

        async Task<AuthResult<AuthResponseDto>> CallRefreshAsync()
        {
            await using var reqCtx = db.NewContext();
            var (reqSut, _, _, _) = CreateSut(reqCtx, t0, sharedStore);
            return await reqSut.RefreshAsync(rawRefresh, CancellationToken.None);
        }

        var results = await Task.WhenAll(Task.Run(CallRefreshAsync), Task.Run(CallRefreshAsync));

        Assert.Equal(1, results.Count(r => r.IsSuccess));
        Assert.Equal(1, results.Count(r => r.Error == AuthError.InvalidRefreshToken));
    }

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task RefreshAsync_revokes_all_when_user_locked_and_RevokeAsync_is_idempotent()
    {
        var db = new TestDb();
        var user = await SeedUserAsync(db, status: UserStatus.Active);
        var t0 = new DateTimeOffset(2026, 11, 28, 10, 0, 0, TimeSpan.Zero);

        await using var ctx = db.NewContext();
        var (sut, store, _, _) = CreateSut(ctx, t0);

        var t1 = await sut.IssueAsync(user, CancellationToken.None);
        var t2 = await sut.IssueAsync(user, CancellationToken.None);

        // Logout t1 idempotent
        await sut.RevokeAsync(t1.RefreshToken, CancellationToken.None);
        await sut.RevokeAsync(t1.RefreshToken, CancellationToken.None);
        await sut.RevokeAsync("unknown-token", CancellationToken.None);
        Assert.Equal(AuthError.InvalidRefreshToken, (await sut.RefreshAsync(t1.RefreshToken, CancellationToken.None)).Error);

        // Khóa tài khoản trong DB -> làm mới t2 thất bại và thu hồi toàn bộ
        await using (var lockCtx = db.NewContext())
        {
            var tracked = await lockCtx.Users.SingleAsync(u => u.Id == user.Id);
            typeof(User).GetProperty(nameof(User.Status))!.SetValue(tracked, UserStatus.Locked);
            await lockCtx.SaveChangesAsync();
        }

        Assert.Equal(AuthError.InvalidRefreshToken, (await sut.RefreshAsync(t2.RefreshToken, CancellationToken.None)).Error);
        Assert.All(store.Tokens, t => Assert.NotNull(t.RevokedAt));
    }
}