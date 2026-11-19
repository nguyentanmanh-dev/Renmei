using Renmei.Domain.Enums;

namespace Renmei.Domain.Content;

public class ImportBatch
{
    private ImportBatch() { }

    public ImportBatch(int sourceId, Guid? runBy, DateTime startedAtUtc)
    {
        Id = Guid.NewGuid();
        SourceId = sourceId;
        RunBy = runBy;
        Status = ImportBatchStatus.Running;
        StartedAt = startedAtUtc;
    }

    public Guid Id { get; private set; }
    public int SourceId { get; private set; }
    public Guid? RunBy { get; private set; }
    public ImportBatchStatus Status { get; private set; }
    public int TotalRows { get; private set; }
    public int InsertedCount { get; private set; }
    public int UpdatedCount { get; private set; }
    public int SkippedCount { get; private set; }
    public int ErrorCount { get; private set; }
    public string? ReportJson { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }
}