using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Content;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class ContentSourceConfiguration : IEntityTypeConfiguration<ContentSource>
{
    public void Configure(EntityTypeBuilder<ContentSource> b)
    {
        b.ToTable("ContentSources");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();

        b.Property(x => x.License).HasMaxLength(100).IsRequired();
        b.Property(x => x.LicenseUrl).HasMaxLength(500);
        b.Property(x => x.AttributionText).HasMaxLength(500);
        b.Property(x => x.Url).HasMaxLength(500);
        b.Property(x => x.LicenseCheckedAt).HasColumnType("datetime2");
    }
}