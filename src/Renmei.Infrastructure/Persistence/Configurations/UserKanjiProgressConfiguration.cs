using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Learning;
using Renmei.Domain.Users;
using Renmei.Domain.Content;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class UserKanjiProgressConfiguration : IEntityTypeConfiguration<UserKanjiProgress>
{
    public void Configure(EntityTypeBuilder<UserKanjiProgress> b)
    {
        b.ToTable("UserKanjiProgress", t =>
        {
            t.HasCheckConstraint("CK_UserKanjiProgress_EaseFactor", "[EaseFactor] >= 1.3");
            t.HasCheckConstraint("CK_UserKanjiProgress_IntervalDays", "[IntervalDays] >= 0");
            t.HasCheckConstraint("CK_UserKanjiProgress_Repetitions", "[Repetitions] >= 0");
            t.HasCheckConstraint("CK_UserKanjiProgress_LapseCount", "[LapseCount] >= 0");
        });

        b.HasKey(x => new { x.UserId, x.KanjiId });
        b.Property(x => x.EaseFactor).HasColumnType("float").HasDefaultValue(2.5);
        b.Property(x => x.DueDate).HasColumnType("date");
        b.Property(x => x.State).HasConversion<string>().HasMaxLength(20).IsRequired().HasDefaultValue(KanjiProgressState.New);

        b.HasIndex(x => new { x.UserId, x.DueDate });

        b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Kanji>().WithMany().HasForeignKey(x => x.KanjiId).OnDelete(DeleteBehavior.Restrict);
    }
}