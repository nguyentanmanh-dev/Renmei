using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Content;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class ExampleWordConfiguration : IEntityTypeConfiguration<ExampleWord>
{
    public void Configure(EntityTypeBuilder<ExampleWord> b)
    {
        b.ToTable("ExampleWords", t => t.HasCheckConstraint("CK_ExampleWords_JlptLevel", "[JlptLevel] IS NULL OR [JlptLevel] BETWEEN 1 AND 5"));
        b.HasKey(x => x.Id);

        b.Property(x => x.Word).HasMaxLength(50).IsRequired();
        b.Property(x => x.Reading).HasMaxLength(100).IsRequired();
        b.Property(x => x.MeaningVi).HasMaxLength(300).IsRequired();
        b.Property(x => x.SourceRef).HasMaxLength(100);

        b.HasIndex(x => x.KanjiId);
        b.HasOne<ContentSource>().WithMany().HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.Restrict);
    }
}