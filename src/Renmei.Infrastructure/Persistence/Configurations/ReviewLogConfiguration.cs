using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Learning;
using Renmei.Domain.Users;
using Renmei.Domain.Content;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class ReviewLogConfiguration : IEntityTypeConfiguration<ReviewLog>
{
    public void Configure(EntityTypeBuilder<ReviewLog> b)
    {
        b.ToTable("ReviewLogs", t =>
        {
            t.HasCheckConstraint("CK_ReviewLogs_Rating", "[Rating] BETWEEN 0 AND 5");
            t.HasCheckConstraint("CK_ReviewLogs_TimeTakenMs", "[TimeTakenMs] IS NULL OR [TimeTakenMs] >= 0");
        });

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();
        b.Property(x => x.PrevEase).HasColumnType("float");
        b.Property(x => x.NewEase).HasColumnType("float");
        b.Property(x => x.ReviewedAt).HasColumnType("datetime2");

        b.HasIndex(x => x.ClientEventId).IsUnique();
        b.HasIndex(x => new { x.UserId, x.ReviewedAt });

        b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Kanji>().WithMany().HasForeignKey(x => x.KanjiId).OnDelete(DeleteBehavior.Restrict);
    }
}