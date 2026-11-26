using Microsoft.EntityFrameworkCore;
using Renmei.Application.Features.Kanji;
using Renmei.Application.Tests.Support;
using Renmei.Domain.Content;
using Renmei.Domain.Enums;

namespace Renmei.Application.Tests.Features.Kanji;

public sealed class KanjiQueryServiceTests
{
    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task ListAsync_returns_only_published_kanji()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            seed.Kanji.AddRange(
                KanjiBuilder.Create("日", status: KanjiStatus.Published),
                KanjiBuilder.Create("月", status: KanjiStatus.Draft),
                KanjiBuilder.Create("火", status: KanjiStatus.InReview),
                KanjiBuilder.Create("水", status: KanjiStatus.Archived));
            await seed.SaveChangesAsync();
        }

        await using var ctx = db.NewContext();
        var result = await new KanjiQueryService(ctx).ListAsync(new ListKanjiRequest(), CancellationToken.None);

        Assert.Equal(["日"], result.Items.Select(i => i.Character));
        Assert.Equal(1, result.TotalItems);
    }

    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task ListAsync_filters_by_level_and_returns_empty_when_no_match()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            seed.Kanji.AddRange(
                KanjiBuilder.Create("日", level: 5),
                KanjiBuilder.Create("学", level: 4));
            await seed.SaveChangesAsync();
        }

        await using var ctx = db.NewContext();
        var sut = new KanjiQueryService(ctx);
        var n5 = await sut.ListAsync(new ListKanjiRequest { Level = 5 }, CancellationToken.None);
        var n1 = await sut.ListAsync(new ListKanjiRequest { Level = 1 }, CancellationToken.None);

        Assert.Equal(["日"], n5.Items.Select(x => x.Character));
        Assert.Empty(n1.Items);
        Assert.Equal(0, n1.TotalItems);
        Assert.Equal(0, n1.TotalPages);
    }

    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task ListAsync_paginates_45_items_without_duplicates_or_omissions_and_handles_out_of_range_page()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            // Sinh 45 ký tự giả từ dải CJK Unified Ideographs cố định (U+4E00..U+4E2C)
            for (var i = 0; i < 45; i++)
            {
                var syntheticChar = char.ConvertFromUtf32(0x4E00 + i);
                seed.Kanji.Add(KanjiBuilder.Create(syntheticChar, level: (i % 5) + 1));
            }

            await seed.SaveChangesAsync();
        }

        var seenIds = new List<int>();
        for (var page = 1; page <= 3; page++)
        {
            await using var ctx = db.NewContext();
            var r = await new KanjiQueryService(ctx).ListAsync(
                new ListKanjiRequest { Page = page, PageSize = 20 },
                CancellationToken.None);

            Assert.Equal(45, r.TotalItems);
            Assert.Equal(3, r.TotalPages);
            Assert.Equal(page < 3 ? 20 : 5, r.Items.Count);
            seenIds.AddRange(r.Items.Select(i => i.Id));
        }

        Assert.Equal(45, seenIds.Count);
        Assert.Equal(45, seenIds.Distinct().Count());

        // Ca 4: Page vượt quá trang cuối
        await using var overflowCtx = db.NewContext();
        var overflow = await new KanjiQueryService(overflowCtx).ListAsync(
            new ListKanjiRequest { Page = 4, PageSize = 20 },
            CancellationToken.None);

        Assert.Empty(overflow.Items);
        Assert.Equal(45, overflow.TotalItems);
    }

    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task ListAsync_orders_deterministically_by_level_desc_then_id_asc()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            seed.Kanji.AddRange(
                KanjiBuilder.Create("一", level: 4),
                KanjiBuilder.Create("三", level: 5),
                KanjiBuilder.Create("二", level: 5));
            await seed.SaveChangesAsync();
        }

        await using var ctx1 = db.NewContext();
        var run1 = await new KanjiQueryService(ctx1).ListAsync(new ListKanjiRequest(), CancellationToken.None);

        await using var ctx2 = db.NewContext();
        var run2 = await new KanjiQueryService(ctx2).ListAsync(new ListKanjiRequest(), CancellationToken.None);

        Assert.Equal(["三", "二", "一"], run1.Items.Select(x => x.Character));
        Assert.Equal(run1.Items.Select(x => x.Id), run2.Items.Select(x => x.Id));
    }

    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task ListAsync_searches_exact_character_for_single_rune_including_surrogate_pair_and_meaning_substring_otherwise()
    {
        const string rareSurrogateKanji = "\U0002000B";

        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            var k1 = KanjiBuilder.Create("日", level: 5, status: KanjiStatus.Published);
            k1.ReplaceMeanings([
                new KanjiMeaning(0, MeaningType.HanViet, "nhật", true, 0),
                new KanjiMeaning(0, MeaningType.Meaning, "mặt trời", false, 1)
            ]);

            var k2 = KanjiBuilder.Create("明", level: 4, status: KanjiStatus.Published);
            k2.ReplaceMeanings([
                new KanjiMeaning(0, MeaningType.Meaning, "ánh sáng mặt trời", true, 0)
            ]);

            var kDraft = KanjiBuilder.Create("陽", level: 3, status: KanjiStatus.Draft);
            kDraft.ReplaceMeanings([
                new KanjiMeaning(0, MeaningType.Meaning, "mặt trời", true, 0)
            ]);

            var kRare = KanjiBuilder.Create(rareSurrogateKanji, level: 1, status: KanjiStatus.Published);
            kRare.ReplaceMeanings([
                new KanjiMeaning(0, MeaningType.Meaning, "nghĩa chứa chữ 日 nhưng không được khớp khi tìm 1 rune", true, 0)
            ]);

            seed.Kanji.AddRange(k1, k2, kDraft, kRare);
            await seed.SaveChangesAsync();
        }

        await using var ctx = db.NewContext();
        var sut = new KanjiQueryService(ctx);

        var bySingleBmp = await sut.ListAsync(new ListKanjiRequest { Q = "日" }, CancellationToken.None);
        Assert.Equal(["日"], bySingleBmp.Items.Select(x => x.Character));

        var bySurrogate = await sut.ListAsync(new ListKanjiRequest { Q = rareSurrogateKanji }, CancellationToken.None);
        Assert.Equal([rareSurrogateKanji], bySurrogate.Items.Select(x => x.Character));

        var byMeaning = await sut.ListAsync(new ListKanjiRequest { Q = "mặt trời" }, CancellationToken.None);
        Assert.Equal(["日", "明"], byMeaning.Items.Select(x => x.Character));
    }

    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task ListAsync_projects_sorted_hanviet_only_or_empty_array_when_none()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            var withHanViet = KanjiBuilder.Create("行", level: 5);
            withHanViet.ReplaceMeanings([
                new KanjiMeaning(0, MeaningType.HanViet, "hàng", false, 1),
                new KanjiMeaning(0, MeaningType.HanViet, "hành", true, 0),
                new KanjiMeaning(0, MeaningType.Meaning, "đi", false, 0)
            ]);

            var withoutHanViet = KanjiBuilder.Create("々", level: 4);
            withoutHanViet.ReplaceMeanings([
                new KanjiMeaning(0, MeaningType.Meaning, "dấu lặp chữ", true, 0)
            ]);

            seed.Kanji.AddRange(withHanViet, withoutHanViet);
            await seed.SaveChangesAsync();
        }

        await using var ctx = db.NewContext();
        var res = await new KanjiQueryService(ctx).ListAsync(new ListKanjiRequest(), CancellationToken.None);

        Assert.Equal(["hành", "hàng"], res.Items[0].HanViet);
        Assert.Empty(res.Items[1].HanViet);
    }

    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task GetAsync_returns_full_detail_for_published_and_null_for_draft_archived_or_missing()
    {
        var db = new TestDb();
        int publishedId, draftId, archivedId;

        await using (var seed = db.NewContext())
        {
            var pub = KanjiBuilder.Create("学", level: 5, status: KanjiStatus.Draft, sourceId: 1, "học");
            pub.UpdateContent(
                jlptLevel: 5,
                strokeCount: 8,
                radical: "子",
                mnemonicVi: "Đứa trẻ dưới mái nhà",
                imageUrl: "https://cdn.renmei.vn/img/gaku.png",
                gifUrl: "https://cdn.renmei.vn/gif/gaku.gif",
                audioUrl: "https://cdn.renmei.vn/audio/gaku.mp3",
                sourceId: 1,
                by: null);
            pub.ReplaceReadings([new KanjiReading(0, ReadingType.On, "ガク", "gaku")]);
            pub.ReplaceExampleWords([new ExampleWord(0, "学生", "がくせい", "học sinh", sourceId: 1)]);
            pub.SetStatus(KanjiStatus.Published);

            var draft = KanjiBuilder.Create("校", status: KanjiStatus.Draft);
            var archived = KanjiBuilder.Create("旧", status: KanjiStatus.Archived);

            seed.Kanji.AddRange(pub, draft, archived);
            await seed.SaveChangesAsync();

            publishedId = pub.Id;
            draftId = draft.Id;
            archivedId = archived.Id;
        }

        await using var ctx = db.NewContext();
        var sut = new KanjiQueryService(ctx);

        var detail = await sut.GetAsync(publishedId, CancellationToken.None);
        var draftDetail = await sut.GetAsync(draftId, CancellationToken.None);
        var archivedDetail = await sut.GetAsync(archivedId, CancellationToken.None);
        var missingDetail = await sut.GetAsync(999999, CancellationToken.None);

        Assert.NotNull(detail);
        Assert.Equal("学", detail.Character);
        Assert.Equal(8, detail.StrokeCount);
        Assert.Equal("子", detail.Radical);
        Assert.Equal("Đứa trẻ dưới mái nhà", detail.MnemonicVi);
        Assert.Equal("https://cdn.renmei.vn/img/gaku.png", detail.MnemonicImageUrl);
        Assert.Equal("https://cdn.renmei.vn/gif/gaku.gif", detail.StrokeOrderGifUrl);
        Assert.Equal("https://cdn.renmei.vn/audio/gaku.mp3", detail.AudioUrl);
        Assert.Single(detail.Readings);
        Assert.Single(detail.Meanings);
        Assert.Single(detail.ExampleWords);

        Assert.Null(draftDetail);
        Assert.Null(archivedDetail);
        Assert.Null(missingDetail);
    }

    [Fact]
    [Trait("Requirement", "FR-005")]
    public async Task Queries_do_not_track_entities_in_change_tracker()
    {
        var db = new TestDb();
        int id;
        await using (var seed = db.NewContext())
        {
            var k = KanjiBuilder.Create("日", status: KanjiStatus.Published, hanViet: "nhật");
            seed.Kanji.Add(k);
            await seed.SaveChangesAsync();
            id = k.Id;
        }

        await using var ctx = db.NewContext();
        var sut = new KanjiQueryService(ctx);
        await sut.ListAsync(new ListKanjiRequest(), CancellationToken.None);
        await sut.GetAsync(id, CancellationToken.None);

        Assert.Empty(ctx.ChangeTracker.Entries());
    }
}