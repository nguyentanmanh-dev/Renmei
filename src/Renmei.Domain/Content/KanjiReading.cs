using Renmei.Domain.Enums;

namespace Renmei.Domain.Content;

public class KanjiReading
{
    private KanjiReading() { }

    public KanjiReading(int kanjiId, ReadingType type, string reading, string? romaji = null, bool isPrimary = false, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(reading)) throw new ArgumentException("Reading is required.", nameof(reading));

        KanjiId = kanjiId;
        Type = type;
        Reading = reading.Trim();
        Romaji = romaji?.Trim();
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
    }

    public int Id { get; private set; }
    public int KanjiId { get; private set; }
    public ReadingType Type { get; private set; }
    public string Reading { get; private set; } = null!;
    public string? Romaji { get; private set; }
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }
}