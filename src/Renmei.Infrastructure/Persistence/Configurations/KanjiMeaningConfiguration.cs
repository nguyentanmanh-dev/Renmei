using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Content;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class KanjiMeaningConfiguration : IEntityTypeConfiguration<KanjiMeaning>
{
    public void Configure(EntityTypeBuilder<KanjiMeaning> b)
    {
        b.ToTable("KanjiMeanings");
        b.HasKey(x => x.Id);

        b.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.Text).HasMaxLength(200).IsRequired();

        b.HasIndex(x => new { x.KanjiId, x.Type });
    }
}