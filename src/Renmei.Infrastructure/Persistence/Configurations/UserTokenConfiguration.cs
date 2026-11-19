using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Users;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> b)
    {
        b.ToTable("UserTokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();

        b.Property(x => x.ExpiresAt).HasColumnType("datetime2");
        b.Property(x => x.UsedAt).HasColumnType("datetime2");

        b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}