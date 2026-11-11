using Importer.Models;

namespace Importer.Validation;

public class KanjiImportValidator
{
    private readonly Dictionary<string, ContentSource> _sources;
    private readonly HashSet<string> _approvedN5Set;
    private readonly HashSet<string> _existingDbKanji;

    public KanjiImportValidator(
        IEnumerable<ContentSource> sources,
        IEnumerable<string> approvedN5List,
        IEnumerable<string> existingDbKanji)
    {
        _sources = sources.ToDictionary(s => s.Id, s => s);
        _approvedN5Set = new HashSet<string>(approvedN5List);
        _existingDbKanji = new HashSet<string>(existingDbKanji);
    }

    public ProcessedKanjiResult ProcessRecord(RawKanjiRecord record, HashSet<string> processedInFile)
    {
        var result = new ProcessedKanjiResult
        {
            Character = record.Character,
            Level = record.JlptLevel ?? 0
        };

        if (!_sources.TryGetValue(record.SourceId, out var source))
        {
            result.Issues.Add("LICENSE_MISSING");
        }
        else if (string.IsNullOrWhiteSpace(source.LicenseCheckedAt))
        {
            result.Issues.Add("LICENSE_UNVERIFIED");
        }

        if (string.IsNullOrWhiteSpace(record.SourceRef) || record.JlptLevel is < 1 or > 5)
        {
            result.Issues.Add("PROVENANCE_MISSING");
        }

        if (processedInFile.Contains(record.Character))
        {
            result.Issues.Add("DUPLICATE_IN_FILE");
        }
        if (_existingDbKanji.Contains(record.Character))
        {
            result.Issues.Add("DUPLICATE_IN_DB");
        }

        if (string.IsNullOrWhiteSpace(record.Character))
            result.Issues.Add("REQUIRED_MISSING:character");
        if (record.Onyomi.Count == 0 && record.Kunyomi.Count == 0)
            result.Issues.Add("REQUIRED_MISSING:readings");
        if (record.MeaningsVi.Count == 0)
            result.Issues.Add("REQUIRED_MISSING:meanings_vi");
        if (record.Examples.Count == 0)
            result.Issues.Add("REQUIRED_MISSING:examples");

        bool hasCriticalError = result.Issues.Any(i =>
            i.StartsWith("LICENSE") || i.StartsWith("PROVENANCE") || i.StartsWith("DUPLICATE"));

        if (hasCriticalError)
        {
            result.Status = "Rejected";
        }
        else if (result.Issues.Any(i => i.StartsWith("REQUIRED_MISSING")))
        {
            result.Status = "Draft";
        }
        else
        {
            if (record.JlptLevel == 5)
            {
                result.Status = _approvedN5Set.Contains(record.Character) ? "Published" : "InReview";
            }
            else
            {
                result.Status = "InReview";
            }
        }

        return result;
    }
}