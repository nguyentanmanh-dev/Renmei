using Microsoft.EntityFrameworkCore;
using Renmei.Domain.Auditing;
using Renmei.Domain.Content;
using Renmei.Domain.Users;
using Renmei.Domain.Learning;

namespace Renmei.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserToken> UserTokens { get; }
    DbSet<ContentSource> ContentSources { get; }
    DbSet<ImportBatch> ImportBatches { get; }
    DbSet<Kanji> Kanji { get; }
    DbSet<KanjiReading> KanjiReadings { get; }
    DbSet<KanjiMeaning> KanjiMeanings { get; }
    DbSet<ExampleWord> ExampleWords { get; }
    DbSet<AuditLog> AuditLogs { get; }

    DbSet<UserKanjiProgress> UserKanjiProgress { get; }
    DbSet<ReviewLog> ReviewLogs { get; }
    DbSet<UserStreak> UserStreaks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}