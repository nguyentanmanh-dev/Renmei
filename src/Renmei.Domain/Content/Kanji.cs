using Renmei.Domain.Enums;

namespace Renmei.Domain.Content;

public class Kanji
{
    private readonly List<KanjiReading> _readings = [];
    private readonly List<KanjiMeaning> _meanings = [];
    private readonly List<ExampleWord> _exampleWords = [];

    private Kanji() { }

    public Kanji(string character, int jlptLevel, int sourceId, Guid? createdBy, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(character)) throw new ArgumentException("Character is required.", nameof(character));
        if (jlptLevel is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(jlptLevel));

        Character = character;
        JlptLevel = jlptLevel;
        SourceId = sourceId;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
        Status = KanjiStatus.Draft;
    }

    public int Id { get; private set; }
    public string Character { get; private set; } = null!;
    public int JlptLevel { get; private set; }
    public int? StrokeCount { get; private set; }
    public string? Radical { get; private set; }
    public string? MnemonicVi { get; private set; }
    public string? MnemonicImageUrl { get; private set; }
    public string? StrokeOrderGifUrl { get; private set; }
    public string? AudioUrl { get; private set; }
    public KanjiStatus Status { get; private set; }
    public int SourceId { get; private set; }
    public string? SourceRef { get; private set; }
    public Guid? ImportBatchId { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public IReadOnlyCollection<KanjiReading> Readings => _readings;
    public IReadOnlyCollection<KanjiMeaning> Meanings => _meanings;
    public IReadOnlyCollection<ExampleWord> ExampleWords => _exampleWords;
}