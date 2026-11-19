using Renmei.Domain.Content;
using Renmei.Domain.Enums;

namespace Renmei.Domain.Tests;

public class KanjiTests
{
    [Fact]
    public void Constructor_ValidArguments_SetsDraftStatusAndProperties()
    {
        var now = new DateTime(2026, 11, 19, 12, 0, 0, DateTimeKind.Utc);
        var userId = Guid.NewGuid();

        var kanji = new Kanji("学", 5, 1, userId, now);

        Assert.Equal("学", kanji.Character);
        Assert.Equal(5, kanji.JlptLevel);
        Assert.Equal(KanjiStatus.Draft, kanji.Status);
        Assert.Equal(userId, kanji.CreatedBy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Constructor_InvalidJlptLevel_ThrowsOutOfRange(int invalidLevel)
    {
        var now = DateTime.UtcNow;
        Assert.Throws<ArgumentOutOfRangeException>(() => new Kanji("学", invalidLevel, 1, null, now));
    }
}