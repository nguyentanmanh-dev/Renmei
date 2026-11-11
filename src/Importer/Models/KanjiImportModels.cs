namespace Importer.Models;

public record ContentSource(
    string Id,
    string Name,
    string Usage,
    string License,
    string LicenseUrl,
    string AttributionText,
    string LicenseCheckedAt
);

public class RawKanjiRecord
{
    public string Character { get; set; } = string.Empty;
    public int? JlptLevel { get; set; }
    public string HanViet { get; set; } = string.Empty;
    public List<string> MeaningsVi { get; set; } = new();
    public List<string> Onyomi { get; set; } = new();
    public List<string> Kunyomi { get; set; } = new();
    public List<string> Examples { get; set; } = new();
    public string SourceId { get; set; } = string.Empty;
    public string SourceRef { get; set; } = string.Empty;
}

public class ProcessedKanjiResult
{
    public string Character { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Status { get; set; } = "Rejected"; 
    public List<string> Issues { get; set; } = new();
}

public class ImportReportSummary
{
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public int TotalRows { get; set; }
    public int InsertedRows { get; set; }
    public int SkippedDuplicates { get; set; }
    public int RejectedRows { get; set; }
    public Dictionary<string, int> StatusCounts { get; set; } = new();
    public List<ProcessedKanjiResult> Details { get; set; } = new();
}