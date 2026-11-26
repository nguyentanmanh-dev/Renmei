using Renmei.Domain.Content;
using Renmei.Domain.Enums;
using Renmei.Domain.Exceptions;

namespace Renmei.Domain.Tests.Content;

public sealed class KanjiDomainTests
{
    private static readonly DateTime FixedNow = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void New_kanji_starts_in_Draft_status_with_expected_level()
    {
        var k = new Kanji("学", 5, 1, null, FixedNow);

        Assert.Equal(KanjiStatus.Draft, k.Status);
        Assert.Equal(5, k.JlptLevel);
        Assert.Equal("学", k.Character);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Constructor_and_UpdateContent_throw_when_jlpt_level_out_of_range(int level)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Kanji("学", level, 1, null, FixedNow));

        var valid = new Kanji("学", 5, 1, null, FixedNow);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            valid.UpdateContent(level, 8, null, null, null, null, null, 1, null));
    }

    [Theory]
    [InlineData(KanjiStatus.Published)]
    [InlineData(KanjiStatus.Archived)]
    public void UpdateContent_throws_DomainException_when_not_editable(KanjiStatus status)
    {
        var k = new Kanji("学", 5, 1, null, FixedNow);
        typeof(Kanji).GetProperty(nameof(Kanji.Status))!.SetValue(k, status);

        var ex = Assert.Throws<DomainException>(() =>
            k.UpdateContent(5, 8, "子", null, null, null, null, 1, null));

        Assert.Equal("kanji-not-editable", ex.Code);
    }

    [Fact]
    public void Archive_is_idempotent()
    {
        var k = new Kanji("学", 5, 1, null, FixedNow);
        k.Archive(null);
        k.Archive(null);

        Assert.Equal(KanjiStatus.Archived, k.Status);
    }

    [Fact]
    public void ReplaceReadings_replaces_existing_collection()
    {
        var k = new Kanji("学", 5, 1, null, FixedNow);
        k.ReplaceReadings([new KanjiReading(0, ReadingType.On, "ガク", "gaku")]);

        k.ReplaceReadings([new KanjiReading(0, ReadingType.Kun, "まな.ぶ", "manabu")]);

        Assert.Single(k.Readings);
        Assert.Equal("まな.ぶ", k.Readings.First().Reading);
    }
}