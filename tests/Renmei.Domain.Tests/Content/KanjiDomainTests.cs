using Renmei.Domain.Content;
using Renmei.Domain.Enums;
using Renmei.Domain.Exceptions;

namespace Renmei.Domain.Tests.Content;

public sealed class KanjiDomainTests
{
    [Theory]
    [InlineData(KanjiStatus.Published)]
    [InlineData(KanjiStatus.Archived)]
    public void UpdateContent_throws_DomainException_when_not_editable(KanjiStatus status)
    {
        var k = new Kanji("学", 5, 1, null, DateTime.UtcNow);
        typeof(Kanji).GetProperty(nameof(Kanji.Status))!.SetValue(k, status);

        var ex = Assert.Throws<DomainException>(() =>
            k.UpdateContent(5, 8, "子", null, null, null, null, 1, null));

        Assert.Equal("kanji-not-editable", ex.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void UpdateContent_throws_when_jlpt_level_out_of_range(int level)
    {
        var k = new Kanji("学", 5, 1, null, DateTime.UtcNow);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            k.UpdateContent(level, 8, null, null, null, null, null, 1, null));
    }

    [Fact]
    public void Archive_is_idempotent()
    {
        var k = new Kanji("学", 5, 1, null, DateTime.UtcNow);
        k.Archive(null);
        k.Archive(null);

        Assert.Equal(KanjiStatus.Archived, k.Status);
    }

    [Fact]
    public void ReplaceReadings_replaces_existing_collection()
    {
        var k = new Kanji("学", 5, 1, null, DateTime.UtcNow);
        k.ReplaceReadings([new KanjiReading(0, ReadingType.On, "ガク", "gaku")]);

        k.ReplaceReadings([new KanjiReading(0, ReadingType.Kun, "まな.ぶ", "manabu")]);

        Assert.Single(k.Readings);
        Assert.Equal("まな.ぶ", k.Readings.First().Reading);
    }
}