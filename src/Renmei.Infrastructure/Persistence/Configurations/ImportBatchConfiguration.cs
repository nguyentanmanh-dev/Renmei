using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmei.Domain.Content;
using Renmei.Domain.Users;

namespace Renmei.Infrastructure.Persistence.Configurations;

public sealed class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
{
    public void Configure(EntityTypeBuilder<ImportBatch> b)
    {
        b.ToTable("ImportBatches");
        b.HasKey(x => x.Id);

        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.ReportJson).HasColumnType("nvarchar(max)");
        b.Property(x => x.StartedAt).HasColumnType("datetime2");
        b.Property(x => x.FinishedAt).HasColumnType("datetime2");

        b.HasOne<ContentSource>().WithMany().HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.RunBy).OnDelete(DeleteBehavior.Restrict);
    }
}