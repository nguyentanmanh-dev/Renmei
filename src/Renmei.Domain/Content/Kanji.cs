using Renmei.Domain.Enums;
using Renmei.Domain.Exceptions;

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

    public bool IsEditable => Status is KanjiStatus.Draft or KanjiStatus.InReview;

    public void UpdateContent(
        int jlptLevel,
        int? strokeCount,
        string? radical,
        string? mnemonicVi,
        string? imageUrl,
        string? gifUrl,
        string? audioUrl,
        int sourceId,
        Guid? by)
    {
        if (!IsEditable)
        {
            throw new DomainException("kanji-not-editable", $"Kanji in status {Status} cannot be edited.");
        }

        if (jlptLevel is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(jlptLevel));
        }

        JlptLevel = jlptLevel;
        StrokeCount = strokeCount;
        Radical = radical;
        MnemonicVi = mnemonicVi;
        MnemonicImageUrl = imageUrl;
        StrokeOrderGifUrl = gifUrl;
        AudioUrl = audioUrl;
        SourceId = sourceId;
        UpdatedBy = by;
    }

    public void ReplaceReadings(IEnumerable<KanjiReading> items)
    {
        _readings.Clear();
        _readings.AddRange(items);
    }

    public void ReplaceMeanings(IEnumerable<KanjiMeaning> items)
    {
        _meanings.Clear();
        _meanings.AddRange(items);
    }

    public void ReplaceExampleWords(IEnumerable<ExampleWord> items)
    {
        _exampleWords.Clear();
        _exampleWords.AddRange(items);
    }

    public void Archive(Guid? by)
    {
        if (Status == KanjiStatus.Archived)
        {
            return;
        }

        Status = KanjiStatus.Archived;
        UpdatedBy = by;
    }
}