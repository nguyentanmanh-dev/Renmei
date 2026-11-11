using Importer.Models;
using Importer.Validation;
using Xunit;

namespace Importer.Tests;

public class KanjiValidatorTests
{
    private readonly List<ContentSource> _validSources = new()
    {
        new("KANJIDIC2", "KANJIDIC2", "core", "CC BY-SA 4.0", "url", "attr", "2026-11-11")
    };

    [Fact]
    public void Should_Flag_LICENSE_UNVERIFIED_When_CheckedAt_Is_Empty()
    {
        var sources = new List<ContentSource>
        {
            new("KANJIDIC2", "KANJIDIC2", "core", "CC BY-SA 4.0", "url", "attr", "")
        };
        var validator = new KanjiImportValidator(sources, new[] { "学" }, Array.Empty<string>());
        var record = GetValidN5Record();

        var result = validator.ProcessRecord(record, new HashSet<string>());

        Assert.Contains("LICENSE_UNVERIFIED", result.Issues);
        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void Should_Assign_Published_Only_For_Approved_N5()
    {
        var validator = new KanjiImportValidator(_validSources, new[] { "学" }, Array.Empty<string>());
        var approvedRecord = GetValidN5Record();
        approvedRecord.Character = "学";

        var unapprovedRecord = GetValidN5Record();
        unapprovedRecord.Character = "校";

        var resultApproved = validator.ProcessRecord(approvedRecord, new HashSet<string>());
        var resultUnapproved = validator.ProcessRecord(unapprovedRecord, new HashSet<string>());

        Assert.Equal("Published", resultApproved.Status);
        Assert.Equal("InReview", resultUnapproved.Status);
    }

    [Fact]
    public void Should_Never_Assign_Published_For_N4_To_N1()
    {
        var validator = new KanjiImportValidator(_validSources, new[] { "猫" }, Array.Empty<string>());
        var n4Record = GetValidN5Record();
        n4Record.Character = "猫";
        n4Record.JlptLevel = 4;

        var result = validator.ProcessRecord(n4Record, new HashSet<string>());

        Assert.Equal("InReview", result.Status);
        Assert.NotEqual("Published", result.Status);
    }

    private RawKanjiRecord GetValidN5Record() => new()
    {
        Character = "学",
        JlptLevel = 5,
        HanViet = "HỌC",
        MeaningsVi = new() { "Học tập" },
        Onyomi = new() { "ガク" },
        Kunyomi = new() { "まな.ぶ" },
        Examples = new() { " me : 学生" },
        SourceId = "KANJIDIC2",
        SourceRef = "KANJIDIC2-ENTRY-001"
    };
}