using Importer.Models;
using Importer.Sources;
using Importer.Validation;
using Xunit;

namespace Importer.Tests;

public class ContentSourceLoaderTests
{
    private const string ValidJson = """
    [
      { "id": "KANJIDIC2", "name": "KANJIDIC2", "usage": "core", "license": "CC BY-SA 4.0",
        "licenseUrl": "u", "attributionText": "a", "url": "u", "licenseCheckedAt": "14-11-2026" },
      { "id": "TANOS_JLPT", "name": "tanos", "usage": "jlpt-level", "license": "chưa xác định",
        "licenseUrl": "", "attributionText": "a", "url": "", "licenseCheckedAt": "" }
    ]
    """;

    [Fact]
    public void Parse_Maps_CamelCase_Fields_Correctly()
    {
        var sources = ContentSourceLoader.Parse(ValidJson);

        Assert.Equal(2, sources.Count);
        Assert.Equal("KANJIDIC2", sources[0].Id);
        Assert.Equal("u", sources[0].Url);
        Assert.Equal("14-11-2026", sources[0].LicenseCheckedAt);
        Assert.Equal("", sources[1].Url);
        Assert.Equal("", sources[1].LicenseCheckedAt);
    }

    [Fact]
    public void Parse_Throws_When_Id_Is_Missing()
    {
        const string json = """[ { "name": "KANJIDIC2", "licenseCheckedAt": "14-11-2026" } ]""";

        var ex = Assert.Throws<InvalidDataException>(() => ContentSourceLoader.Parse(json));
        Assert.Contains("thiếu trường 'id'", ex.Message);
    }

    [Fact]
    public void Parse_Throws_When_Id_Is_Duplicated()
    {
        const string json = """
        [ { "id": "KANJIDIC2", "name": "A", "licenseCheckedAt": "10-11-2026" },
          { "id": "kanjidic2", "name": "B", "licenseCheckedAt": "14-11-2026" } ]
        """;

        var ex = Assert.Throws<InvalidDataException>(() => ContentSourceLoader.Parse(json));
        Assert.Contains("bị trùng", ex.Message);
    }

    [Fact]
    public void Unverified_Source_Loads_But_Records_From_It_Are_Rejected()
    {
        var sources = ContentSourceLoader.Parse(ValidJson);
        var validator = new KanjiImportValidator(sources, new[] { "学" }, Array.Empty<string>());

        var record = new RawKanjiRecord
        {
            Character = "学",
            JlptLevel = 5,
            HanViet = "học",
            MeaningsVi = new() { "học" },
            Onyomi = new() { "ガク" },
            Examples = new() { "学生" },
            SourceId = "TANOS_JLPT",
            SourceRef = "TANOS-学"
        };

        var result = validator.ProcessRecord(record, new HashSet<string>());

        Assert.Contains("LICENSE_UNVERIFIED", result.Issues);
        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void Parse_Defaults_Url_To_Empty_When_Omitted()
    {
        const string json = """
        [ { "id": "KANJIDIC2", "name": "KANJIDIC2", "licenseCheckedAt": "14-11-2026" } ]
        """;

        var sources = ContentSourceLoader.Parse(json);

        Assert.Single(sources);
        Assert.Equal("", sources[0].Url);
    }

    [Fact]
    public void SourceId_Lookup_Is_Case_Insensitive()
    {
        var sources = ContentSourceLoader.Parse(ValidJson);
        var validator = new KanjiImportValidator(sources, new[] { "学" }, Array.Empty<string>());

        var record = new RawKanjiRecord
        {
            Character = "学",
            JlptLevel = 5,
            HanViet = "học",
            MeaningsVi = new() { "học" },
            Onyomi = new() { "ガク" },
            Examples = new() { "学生" },
            SourceId = "kanjidic2",
            SourceRef = "KANJIDIC2-学"
        };

        var result = validator.ProcessRecord(record, new HashSet<string>());

        Assert.DoesNotContain("LICENSE_MISSING", result.Issues);
        Assert.Equal("Published", result.Status);
    }
}