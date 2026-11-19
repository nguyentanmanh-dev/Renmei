using Renmei.Domain.Enums;

namespace Renmei.Domain.Content;

public class KanjiMeaning
{
    private KanjiMeaning() { }

    public KanjiMeaning(int kanjiId, MeaningType type, string text, bool isPrimary = false, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Meaning text is required.", nameof(text));

        KanjiId = kanjiId;
        Type = type;
        Text = text.Trim();
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
    }

    public int Id { get; private set; }
    public int KanjiId { get; private set; }
    public MeaningType Type { get; private set; }
    public string Text { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }
}