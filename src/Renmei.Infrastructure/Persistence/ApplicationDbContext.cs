using Microsoft.EntityFrameworkCore;
using Renmei.Application.Abstractions;
using Renmei.Domain.Auditing;
using Renmei.Domain.Content;
using Renmei.Domain.Users;
using Renmei.Domain.Learning;

namespace Renmei.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserToken> UserTokens => Set<UserToken>();
    public DbSet<ContentSource> ContentSources => Set<ContentSource>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();
    public DbSet<Kanji> Kanji => Set<Kanji>();
    public DbSet<KanjiReading> KanjiReadings => Set<KanjiReading>();
    public DbSet<KanjiMeaning> KanjiMeanings => Set<KanjiMeaning>();
    public DbSet<ExampleWord> ExampleWords => Set<ExampleWord>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<UserKanjiProgress> UserKanjiProgress => Set<UserKanjiProgress>();
    public DbSet<ReviewLog> ReviewLogs => Set<ReviewLog>();
    public DbSet<UserStreak> UserStreaks => Set<UserStreak>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}