using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Users;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class UserStreakConfiguration : IEntityTypeConfiguration<UserStreak>
{
    public void Configure(EntityTypeBuilder<UserStreak> b)
    {
        b.ToTable("UserStreaks", t =>
        {
            t.HasCheckConstraint("CK_UserStreaks_Current", "[CurrentStreak] >= 0");
            t.HasCheckConstraint("CK_UserStreaks_Longest", "[LongestStreak] >= [CurrentStreak]");
            t.HasCheckConstraint("CK_UserStreaks_Freeze", "[StreakFreezeCount] >= 0");
        });

        b.HasKey(x => x.UserId);
        b.Property(x => x.LastStudyDate).HasColumnType("date");
        b.Property(x => x.LastFreezeGrantMonth).HasColumnType("date");

        b.HasOne<User>().WithOne().HasForeignKey<UserStreak>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}