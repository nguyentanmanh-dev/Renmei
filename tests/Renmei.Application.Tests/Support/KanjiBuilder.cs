using Renmei.Domain.Content;
using Renmei.Domain.Enums;

namespace Renmei.Application.Tests.Support;

internal static class KanjiBuilder
{
    public static readonly DateTime FixedNow = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Kanji Create(
        string character,
        int level = 5,
        KanjiStatus status = KanjiStatus.Published,
        int sourceId = 1,
        params string[] hanViet)
    {
        var k = new Kanji(character, level, sourceId, null, FixedNow);
        k.SetStatus(status);

        var order = 0;
        k.ReplaceMeanings(hanViet.Select(h =>
            new KanjiMeaning(0, MeaningType.HanViet, h, isPrimary: order == 0, sortOrder: order++)));

        return k;
    }
}