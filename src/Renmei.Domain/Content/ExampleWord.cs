namespace Renmei.Domain.Content;

public class ExampleWord
{
    private ExampleWord() { }

    public ExampleWord(
        int kanjiId,
        string word,
        string reading,
        string meaningVi,
        int sourceId,
        int? jlptLevel = null,
        string? sourceRef = null,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(word)) throw new ArgumentException("Word is required.", nameof(word));
        if (string.IsNullOrWhiteSpace(reading)) throw new ArgumentException("Reading is required.", nameof(reading));
        if (string.IsNullOrWhiteSpace(meaningVi)) throw new ArgumentException("MeaningVi is required.", nameof(meaningVi));
        if (jlptLevel is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(jlptLevel));

        KanjiId = kanjiId;
        Word = word.Trim();
        Reading = reading.Trim();
        MeaningVi = meaningVi.Trim();
        SourceId = sourceId;
        JlptLevel = jlptLevel;
        SourceRef = sourceRef;
        SortOrder = sortOrder;
    }

    public int Id { get; private set; }
    public int KanjiId { get; private set; }
    public string Word { get; private set; } = null!;
    public string Reading { get; private set; } = null!;
    public string MeaningVi { get; private set; } = null!;
    public int? JlptLevel { get; private set; }
    public int SourceId { get; private set; }
    public string? SourceRef { get; private set; }
    public int SortOrder { get; private set; }
}