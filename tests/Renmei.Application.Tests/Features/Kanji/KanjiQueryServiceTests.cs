using Microsoft.EntityFrameworkCore;
using Renmei.Application.Features.Kanji;
using Renmei.Domain.Content;
using Renmei.Domain.Enums;
using Renmei.Infrastructure.Persistence;

namespace Renmei.Application.Tests.Features.Kanji;

public sealed class KanjiQueryServiceTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly KanjiQueryService _sut;

    public KanjiQueryServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);
        _sut = new KanjiQueryService(_db);
    }

    public void Dispose() => _db.Dispose();

    private static void SetStatus(Domain.Content.Kanji k, KanjiStatus status)
        => typeof(Domain.Content.Kanji).GetProperty(nameof(Domain.Content.Kanji.Status))!.SetValue(k, status);

    private async Task<Domain.Content.Kanji> SeedKanjiAsync(
        string character,
        int level,
        KanjiStatus status,
        params (MeaningType Type, string Text, int Order)[] meanings)
    {
        var k = new Domain.Content.Kanji(character, level, 1, null, DateTime.UtcNow);
        SetStatus(k, status);

        _db.Kanji.Add(k);
        await _db.SaveChangesAsync();

        foreach (var (type, text, order) in meanings)
        {
            _db.Set<KanjiMeaning>().Add(new KanjiMeaning(k.Id, type, text, isPrimary: false, sortOrder: order));
        }

        if (meanings.Length > 0)
        {
            await _db.SaveChangesAsync();
        }

        return k;
    }

    [Fact]
    public async Task ListAsync_returns_only_published_kanji()
    {
        await SeedKanjiAsync("日", 5, KanjiStatus.Published);
        await SeedKanjiAsync("月", 5, KanjiStatus.Draft);
        await SeedKanjiAsync("火", 5, KanjiStatus.InReview);
        await SeedKanjiAsync("水", 5, KanjiStatus.Archived);

        var res = await _sut.ListAsync(new ListKanjiRequest(), CancellationToken.None);

        Assert.Equal(1, res.TotalItems);
        Assert.Single(res.Items);
        Assert.Equal("日", res.Items[0].Character);
    }

    [Fact]
    public async Task ListAsync_filters_by_level_and_returns_empty_when_no_match()
    {
        await SeedKanjiAsync("日", 5, KanjiStatus.Published);
        await SeedKanjiAsync("学", 4, KanjiStatus.Published);

        var n5 = await _sut.ListAsync(new ListKanjiRequest { Level = 5 }, CancellationToken.None);
        var n1 = await _sut.ListAsync(new ListKanjiRequest { Level = 1 }, CancellationToken.None);

        Assert.Single(n5.Items);
        Assert.Equal("日", n5.Items[0].Character);
        Assert.Empty(n1.Items);
        Assert.Equal(0, n1.TotalItems);
        Assert.Equal(0, n1.TotalPages);
    }

    [Fact]
    public async Task ListAsync_paginates_45_items_without_duplicates_or_omissions()
    {
        for (var i = 0; i < 45; i++)
        {
            var ch = char.ConvertFromUtf32(0x4E00 + i);
            var k = new Domain.Content.Kanji(ch, (i % 5) + 1, 1, null, DateTime.UtcNow);
            SetStatus(k, KanjiStatus.Published);
            _db.Kanji.Add(k);
        }

        await _db.SaveChangesAsync();

        var p1 = await _sut.ListAsync(new ListKanjiRequest { Page = 1, PageSize = 20 }, CancellationToken.None);
        var p2 = await _sut.ListAsync(new ListKanjiRequest { Page = 2, PageSize = 20 }, CancellationToken.None);
        var p3 = await _sut.ListAsync(new ListKanjiRequest { Page = 3, PageSize = 20 }, CancellationToken.None);
        var p4 = await _sut.ListAsync(new ListKanjiRequest { Page = 4, PageSize = 20 }, CancellationToken.None);

        Assert.Equal(45, p1.TotalItems);
        Assert.Equal(3, p1.TotalPages);
        Assert.Equal(20, p1.Items.Count);
        Assert.Equal(20, p2.Items.Count);
        Assert.Equal(5, p3.Items.Count);
        Assert.Empty(p4.Items);
        Assert.Equal(45, p4.TotalItems);

        var allIds = p1.Items.Concat(p2.Items).Concat(p3.Items).Select(x => x.Id).ToList();
        Assert.Equal(45, allIds.Distinct().Count());
    }

    [Fact]
    public async Task ListAsync_orders_deterministically_by_level_desc_then_id_asc()
    {
        await SeedKanjiAsync("一", 4, KanjiStatus.Published);
        await SeedKanjiAsync("二", 5, KanjiStatus.Published);
        await SeedKanjiAsync("三", 5, KanjiStatus.Published);

        var run1 = await _sut.ListAsync(new ListKanjiRequest(), CancellationToken.None);
        var run2 = await _sut.ListAsync(new ListKanjiRequest(), CancellationToken.None);

        Assert.Equal(["二", "三", "一"], run1.Items.Select(x => x.Character));
        Assert.Equal(run1.Items.Select(x => x.Id), run2.Items.Select(x => x.Id));
    }

    [Fact]
    public async Task ListAsync_searches_exact_character_for_single_rune_and_meaning_substring_otherwise()
    {
        await SeedKanjiAsync("日", 5, KanjiStatus.Published, (MeaningType.HanViet, "nhật", 0), (MeaningType.Meaning, "mặt trời", 1));
        await SeedKanjiAsync("明", 4, KanjiStatus.Published, (MeaningType.HanViet, "minh", 0), (MeaningType.Meaning, "ánh sáng mặt trời", 1));
        await SeedKanjiAsync("陽", 3, KanjiStatus.Draft, (MeaningType.Meaning, "mặt trời", 0));

        var byRune = await _sut.ListAsync(new ListKanjiRequest { Q = "日" }, CancellationToken.None);
        var byMeaning = await _sut.ListAsync(new ListKanjiRequest { Q = "mặt trời" }, CancellationToken.None);

        Assert.Single(byRune.Items);
        Assert.Equal("日", byRune.Items[0].Character);
        Assert.Equal(2, byMeaning.Items.Count);
    }

    [Fact]
    public async Task ListAsync_projects_only_hanviet_meanings_sorted_by_sortOrder()
    {
        await SeedKanjiAsync(
            "行",
            5,
            KanjiStatus.Published,
            (MeaningType.HanViet, "hàng", 1),
            (MeaningType.HanViet, "hành", 0),
            (MeaningType.Meaning, "đi", 0));

        var res = await _sut.ListAsync(new ListKanjiRequest(), CancellationToken.None);

        Assert.Equal(["hành", "hàng"], res.Items[0].HanViet);
    }

    [Fact]
    public async Task GetAsync_returns_detail_for_published_and_null_for_draft_or_missing()
    {
        var pub = await SeedKanjiAsync("学", 5, KanjiStatus.Published, (MeaningType.HanViet, "học", 0));
        _db.Set<KanjiReading>().Add(new KanjiReading(pub.Id, ReadingType.On, "ガク", "gaku"));
        _db.Set<ExampleWord>().Add(new ExampleWord(pub.Id, "学生", "がくせい", "học sinh", sourceId: 1));
        await _db.SaveChangesAsync();

        var draft = await SeedKanjiAsync("校", 5, KanjiStatus.Draft);
        _db.ChangeTracker.Clear();

        var detail = await _sut.GetAsync(pub.Id, CancellationToken.None);
        var draftDetail = await _sut.GetAsync(draft.Id, CancellationToken.None);
        var missingDetail = await _sut.GetAsync(999999, CancellationToken.None);

        Assert.NotNull(detail);
        Assert.Equal("学", detail.Character);
        Assert.Single(detail.Readings);
        Assert.Single(detail.Meanings);
        Assert.Single(detail.ExampleWords);
        Assert.Null(draftDetail);
        Assert.Null(missingDetail);
        Assert.Empty(_db.ChangeTracker.Entries());
    }
}