using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Content;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class KanjiReadingConfiguration : IEntityTypeConfiguration<KanjiReading>
{
    public void Configure(EntityTypeBuilder<KanjiReading> b)
    {
        b.ToTable("KanjiReadings");
        b.HasKey(x => x.Id);

        b.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.Reading).HasMaxLength(50).IsRequired();
        b.Property(x => x.Romaji).HasMaxLength(50);

        b.HasIndex(x => new { x.KanjiId, x.Type });
    }
}