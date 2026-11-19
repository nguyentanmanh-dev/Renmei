using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Content;
using Renmei.Domain.Users;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class KanjiConfiguration : IEntityTypeConfiguration<Kanji>
{
    public void Configure(EntityTypeBuilder<Kanji> b)
    {
        b.ToTable("Kanji", t => t.HasCheckConstraint("CK_Kanji_JlptLevel", "[JlptLevel] BETWEEN 1 AND 5"));
        b.HasKey(x => x.Id);

        b.Property(x => x.Character).HasMaxLength(4).IsRequired()
            .UseCollation("Latin1_General_100_BIN2");
        b.HasIndex(x => x.Character).IsUnique();

        b.Property(x => x.Radical).HasMaxLength(4);
        b.Property(x => x.MnemonicVi).HasMaxLength(500);
        b.Property(x => x.MnemonicImageUrl).HasMaxLength(500);
        b.Property(x => x.StrokeOrderGifUrl).HasMaxLength(500);
        b.Property(x => x.AudioUrl).HasMaxLength(500);
        b.Property(x => x.SourceRef).HasMaxLength(100);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        b.Property(x => x.ReviewedAt).HasColumnType("datetime2");
        b.Property(x => x.PublishedAt).HasColumnType("datetime2");

        b.HasIndex(x => new { x.JlptLevel, x.Status });
        b.Property(x => x.RowVersion).IsRowVersion();

        b.HasOne<ContentSource>().WithMany().HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<ImportBatch>().WithMany().HasForeignKey(x => x.ImportBatchId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.ReviewedBy).OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Readings).WithOne().HasForeignKey(r => r.KanjiId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Meanings).WithOne().HasForeignKey(m => m.KanjiId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.ExampleWords).WithOne().HasForeignKey(e => e.KanjiId).OnDelete(DeleteBehavior.Cascade);
    }
}