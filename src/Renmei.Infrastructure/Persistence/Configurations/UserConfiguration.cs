using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Users;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users", t => t.HasCheckConstraint("CK_Users_CurrentLevel", "[CurrentLevel] IS NULL OR [CurrentLevel] BETWEEN 1 AND 5"));
        b.HasKey(x => x.Id);

        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(100);
        b.Property(x => x.TimeZone).HasMaxLength(64).IsRequired();
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        b.Property(x => x.CreatedAt).HasColumnType("datetime2");
        b.Property(x => x.LastLoginAt).HasColumnType("datetime2");
        b.Property(x => x.DeletedAt).HasColumnType("datetime2");
    }
}