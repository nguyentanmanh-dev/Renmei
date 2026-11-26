using Microsoft.EntityFrameworkCore;
using Renmei.Application.Features.Kanji;
using Renmei.Application.Features.KanjiAdmin;
using Renmei.Application.Tests.Support;
using Renmei.Domain.Content;
using Renmei.Domain.Enums;

namespace Renmei.Application.Tests.Features.Kanji;

public sealed class AdminKanjiServiceTests
{
    private static async Task<ContentSource> SeedDefaultSourceAsync(TestDb db)
    {
        await using var seed = db.NewContext();
        var src = new ContentSource("Tự biên soạn", "CC-BY-4.0", "Renmei Team", null);
        seed.ContentSources.Add(src);
        await seed.SaveChangesAsync();
        return src;
    }

    private static SaveKanjiRequest SampleRequest(
        string character = "学",
        int jlptLevel = 5,
        string? mnemonic = "Đứa trẻ dưới mái nhà đang học bài",
        string? imageUrl = null,
        string? gifUrl = null,
        string? audioUrl = null,
        int? sourceId = null) => new(
        Character: character,
        JlptLevel: jlptLevel,
        StrokeCount: 8,
        Radical: "子",
        MnemonicVi: mnemonic,
        MnemonicImageUrl: imageUrl,
        StrokeOrderGifUrl: gifUrl,
        AudioUrl: audioUrl,
        SourceId: sourceId,
        Readings: [new SaveReadingRequest(ReadingType.On, "ガク", "gaku")],
        Meanings: [new SaveMeaningRequest("vi", MeaningType.HanViet, "học", 0)],
        ExampleWords: [new SaveExampleWordRequest("学生", "がくせい", "học sinh", null)]);

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task CreateAsync_creates_draft_with_children_and_default_source()
    {
        var db = new TestDb();
        var src = await SeedDefaultSourceAsync(db);

        await using var ctx = db.NewContext();
        var sut = new AdminKanjiService(ctx, new StubCurrentUser());
        var res = await sut.CreateAsync(SampleRequest(), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Value);
        Assert.Equal(KanjiStatus.Draft, res.Value.Status);
        Assert.Equal(src.Id, res.Value.SourceId);
        Assert.Single(res.Value.Readings);
        Assert.Single(res.Value.Meanings);
        Assert.Single(res.Value.ExampleWords);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task CreateAsync_returns_Duplicate_when_character_already_exists()
    {
        var db = new TestDb();
        await SeedDefaultSourceAsync(db);

        await using (var ctx1 = db.NewContext())
        {
            await new AdminKanjiService(ctx1, new StubCurrentUser()).CreateAsync(SampleRequest("学"), CancellationToken.None);
        }

        await using var ctx2 = db.NewContext();
        var dup = await new AdminKanjiService(ctx2, new StubCurrentUser()).CreateAsync(SampleRequest("学"), CancellationToken.None);

        Assert.Equal(KanjiError.Duplicate, dup.Error);
    }

    [Theory]
    [Trait("Requirement", "FR-010")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("学生")]
    [InlineData("学 校")]
    public async Task CreateAsync_returns_Invalid_when_character_is_empty_whitespace_or_multiple_runes(string badChar)
    {
        var db = new TestDb();
        await SeedDefaultSourceAsync(db);

        await using var ctx = db.NewContext();
        var res = await new AdminKanjiService(ctx, new StubCurrentUser()).CreateAsync(SampleRequest(badChar), CancellationToken.None);

        Assert.Equal(KanjiError.Invalid, res.Error);
    }

    [Theory]
    [Trait("Requirement", "FR-010")]
    [InlineData(0)]
    [InlineData(6)]
    public async Task CreateAsync_returns_Invalid_when_jlpt_level_out_of_range(int invalidLevel)
    {
        var db = new TestDb();
        await SeedDefaultSourceAsync(db);

        await using var ctx = db.NewContext();
        var res = await new AdminKanjiService(ctx, new StubCurrentUser())
            .CreateAsync(SampleRequest(jlptLevel: invalidLevel), CancellationToken.None);

        Assert.Equal(KanjiError.Invalid, res.Error);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task CreateAsync_returns_Invalid_for_unsafe_or_overlong_media_urls()
    {
        var db = new TestDb();
        await SeedDefaultSourceAsync(db);

        var overlongUrl = "https://cdn.renmei.vn/" + new string('a', 500);
        var badRequests = new[]
        {
            SampleRequest(imageUrl: "javascript:alert(1)"),
            SampleRequest(gifUrl: "data:text/html;base64,PHNjcmlwdD4="),
            SampleRequest(audioUrl: overlongUrl)
        };

        foreach (var req in badRequests)
        {
            await using var ctx = db.NewContext();
            var res = await new AdminKanjiService(ctx, new StubCurrentUser()).CreateAsync(req, CancellationToken.None);
            Assert.Equal(KanjiError.Invalid, res.Error);
        }
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task CreateAsync_returns_SourceMissing_when_default_source_is_not_seeded()
    {
        var db = new TestDb();

        await using var ctx = db.NewContext();
        var res = await new AdminKanjiService(ctx, new StubCurrentUser()).CreateAsync(SampleRequest(), CancellationToken.None);

        Assert.Equal(KanjiError.SourceMissing, res.Error);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task UpdateAsync_with_matching_version_replaces_children_completely()
    {
        var db = new TestDb();
        await SeedDefaultSourceAsync(db);

        int id;
        byte[] seededVersion = [1, 2, 3, 4, 5, 6, 7, 8];

        await using (var seed = db.NewContext())
        {
            var k = KanjiBuilder.Create("学", level: 5, status: KanjiStatus.Draft, sourceId: 1, "học");
            k.SetRowVersion(seededVersion);
            k.ReplaceReadings([new KanjiReading(0, ReadingType.On, "ガク", "gaku")]);
            k.ReplaceExampleWords([new ExampleWord(0, "学生", "がくせい", "học sinh", sourceId: 1)]);
            seed.Kanji.Add(k);
            await seed.SaveChangesAsync();
            id = k.Id;
        }

        byte[] currentVersion;
        await using (var readCtx = db.NewContext())
        {
            var detail = await new AdminKanjiService(readCtx, new StubCurrentUser()).GetDetailAsync(id, CancellationToken.None);
            Assert.NotNull(detail);
            currentVersion = detail.RowVersion;
        }

        await using (var updateCtx = db.NewContext())
        {
            var updateReq = SampleRequest("学", jlptLevel: 4, mnemonic: "Đã cập nhật") with
            {
                Readings = [new SaveReadingRequest(ReadingType.Kun, "まな.ぶ", "manabu")],
                Meanings = [new SaveMeaningRequest("vi", MeaningType.Meaning, "học tập", 0)],
                ExampleWords = [new SaveExampleWordRequest("学校", "がっこう", "trường học", null)]
            };

            var res = await new AdminKanjiService(updateCtx, new StubCurrentUser())
                .UpdateAsync(id, currentVersion, updateReq, CancellationToken.None);

            Assert.True(res.IsSuccess);
        }

        await using var verifyCtx = db.NewContext();
        var reloaded = await new AdminKanjiService(verifyCtx, new StubCurrentUser()).GetDetailAsync(id, CancellationToken.None);

        Assert.NotNull(reloaded);
        Assert.Equal(4, reloaded.JlptLevel);
        Assert.Equal("Đã cập nhật", reloaded.MnemonicVi);
        Assert.Single(reloaded.Readings);
        Assert.Equal("まな.ぶ", reloaded.Readings[0].Kana);
        Assert.Single(reloaded.Meanings);
        Assert.Equal("học tập", reloaded.Meanings[0].Text);
        Assert.Single(reloaded.ExampleWords);
        Assert.Equal("学校", reloaded.ExampleWords[0].Word);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task UpdateAsync_with_stale_version_fails_and_leaves_data_unchanged()
    {
        var db = new TestDb();
        int id;
        await using (var seed = db.NewContext())
        {
            var k = KanjiBuilder.Create("日", status: KanjiStatus.Draft);
            k.SetRowVersion([1, 2, 3, 4, 5, 6, 7, 8]);
            seed.Kanji.Add(k);
            await seed.SaveChangesAsync();
            id = k.Id;
        }

        await using (var ctx = db.NewContext())
        {
            var sut = new AdminKanjiService(ctx, new StubCurrentUser());
            var result = await sut.UpdateAsync(id, [9, 9, 9, 9, 9, 9, 9, 9], SampleRequest("日", mnemonic: "ĐÃ SỬA"), CancellationToken.None);
            Assert.Equal(KanjiError.PreconditionFailed, result.Error);
        }

        await using var check = db.NewContext();
        Assert.NotEqual("ĐÃ SỬA", (await check.Kanji.AsNoTracking().SingleAsync()).MnemonicVi);
    }

    [Theory]
    [Trait("Requirement", "FR-010")]
    [InlineData(KanjiStatus.Published)]
    [InlineData(KanjiStatus.Archived)]
    public async Task UpdateAsync_returns_NotEditable_when_kanji_is_published_or_archived(KanjiStatus lockedStatus)
    {
        var db = new TestDb();
        await SeedDefaultSourceAsync(db);

        int id;
        byte[] version = [1, 2, 3, 4, 5, 6, 7, 8];
        await using (var seed = db.NewContext())
        {
            var k = KanjiBuilder.Create("日", status: lockedStatus);
            k.SetRowVersion(version);
            seed.Kanji.Add(k);
            await seed.SaveChangesAsync();
            id = k.Id;
        }

        await using var ctx = db.NewContext();
        var res = await new AdminKanjiService(ctx, new StubCurrentUser())
            .UpdateAsync(id, version, SampleRequest("日"), CancellationToken.None);

        Assert.Equal(KanjiError.NotEditable, res.Error);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task UpdateAsync_returns_Invalid_when_attempting_to_change_character()
    {
        var db = new TestDb();
        await SeedDefaultSourceAsync(db);

        int id;
        byte[] version = [1, 2, 3, 4, 5, 6, 7, 8];
        await using (var seed = db.NewContext())
        {
            var k = KanjiBuilder.Create("日", status: KanjiStatus.Draft);
            k.SetRowVersion(version);
            seed.Kanji.Add(k);
            await seed.SaveChangesAsync();
            id = k.Id;
        }

        await using var ctx = db.NewContext();
        var res = await new AdminKanjiService(ctx, new StubCurrentUser())
            .UpdateAsync(id, version, SampleRequest("月"), CancellationToken.None);

        Assert.Equal(KanjiError.Invalid, res.Error);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task UpdateAsync_returns_NotFound_when_id_does_not_exist()
    {
        var db = new TestDb();

        await using var ctx = db.NewContext();
        var res = await new AdminKanjiService(ctx, new StubCurrentUser())
            .UpdateAsync(999999, [1, 2, 3, 4, 5, 6, 7, 8], SampleRequest("日"), CancellationToken.None);

        Assert.Equal(KanjiError.NotFound, res.Error);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task ListAsync_returns_all_statuses_filters_accurately_and_paginates_without_duplicates()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            seed.Kanji.AddRange(
                KanjiBuilder.Create("日", level: 5, status: KanjiStatus.Draft),
                KanjiBuilder.Create("月", level: 5, status: KanjiStatus.InReview),
                KanjiBuilder.Create("火", level: 4, status: KanjiStatus.Published),
                KanjiBuilder.Create("水", level: 3, status: KanjiStatus.Archived));

            for (var i = 0; i < 41; i++)
            {
                var syntheticChar = char.ConvertFromUtf32(0x4E00 + i);
                seed.Kanji.Add(KanjiBuilder.Create(syntheticChar, level: (i % 5) + 1, status: KanjiStatus.Draft));
            }

            await seed.SaveChangesAsync();
        }

        await using (var filterCtx = db.NewContext())
        {
            var sut = new AdminKanjiService(filterCtx, new StubCurrentUser());
            var all = await sut.ListAsync(new ListAdminKanjiRequest(), CancellationToken.None);
            var onlyArchived = await sut.ListAsync(new ListAdminKanjiRequest { Status = KanjiStatus.Archived }, CancellationToken.None);
            var level3 = await sut.ListAsync(new ListAdminKanjiRequest { Level = 3, Status = KanjiStatus.Archived }, CancellationToken.None);

            Assert.Equal(45, all.TotalItems);
            Assert.Equal(["水"], onlyArchived.Items.Select(x => x.Character));
            Assert.Single(level3.Items);
        }

        var seenIds = new List<int>();
        for (var page = 1; page <= 3; page++)
        {
            await using var pageCtx = db.NewContext();
            var r = await new AdminKanjiService(pageCtx, new StubCurrentUser())
                .ListAsync(new ListAdminKanjiRequest { Page = page, PageSize = 20 }, CancellationToken.None);
            seenIds.AddRange(r.Items.Select(x => x.Id));
        }

        Assert.Equal(45, seenIds.Count);
        Assert.Equal(45, seenIds.Distinct().Count());
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task ListAsync_sets_media_presence_flags_accurately()
    {
        var db = new TestDb();
        await using (var seed = db.NewContext())
        {
            var withMedia = KanjiBuilder.Create("学", level: 5, status: KanjiStatus.Draft);
            withMedia.UpdateContent(5, 8, "子", null, "https://cdn.renmei.vn/img.png", null, "https://cdn.renmei.vn/a.mp3", 1, null);

            var withoutMedia = KanjiBuilder.Create("日", level: 5, status: KanjiStatus.Draft);

            seed.Kanji.AddRange(withMedia, withoutMedia);
            await seed.SaveChangesAsync();
        }

        await using var ctx = db.NewContext();
        var res = await new AdminKanjiService(ctx, new StubCurrentUser()).ListAsync(new ListAdminKanjiRequest(), CancellationToken.None);

        var first = res.Items.Single(x => x.Character == "学");
        var second = res.Items.Single(x => x.Character == "日");

        Assert.True(first.HasImage);
        Assert.False(first.HasStrokeAnimation);
        Assert.True(first.HasAudio);

        Assert.False(second.HasImage);
        Assert.False(second.HasStrokeAnimation);
        Assert.False(second.HasAudio);
    }

    [Fact]
    [Trait("Requirement", "FR-010")]
    public async Task ArchiveAsync_is_idempotent_hides_kanji_from_learner_api_and_rejects_stale_version()
    {
        var db = new TestDb();
        int id;
        byte[] version = [1, 2, 3, 4, 5, 6, 7, 8];

        await using (var seed = db.NewContext())
        {
            var k = KanjiBuilder.Create("火", level: 5, status: KanjiStatus.Published);
            k.SetRowVersion(version);
            seed.Kanji.Add(k);
            await seed.SaveChangesAsync();
            id = k.Id;
        }

        await using (var staleCtx = db.NewContext())
        {
            var staleRes = await new AdminKanjiService(staleCtx, new StubCurrentUser())
                .ArchiveAsync(id, [9, 9, 9, 9, 9, 9, 9, 9], CancellationToken.None);
            Assert.Equal(KanjiError.PreconditionFailed, staleRes.Error);
        }

        await using (var archiveCtx1 = db.NewContext())
        {
            var res1 = await new AdminKanjiService(archiveCtx1, new StubCurrentUser())
                .ArchiveAsync(id, version, CancellationToken.None);
            Assert.True(res1.IsSuccess);
            Assert.Equal(KanjiStatus.Archived, res1.Value!.Status);
        }

        await using (var archiveCtx2 = db.NewContext())
        {
            var res2 = await new AdminKanjiService(archiveCtx2, new StubCurrentUser())
                .ArchiveAsync(id, version, CancellationToken.None);
            Assert.True(res2.IsSuccess);
            Assert.Equal(KanjiStatus.Archived, res2.Value!.Status);
        }

        await using var learnerCtx = db.NewContext();
        var learnerSvc = new KanjiQueryService(learnerCtx);
        var detail = await learnerSvc.GetAsync(id, CancellationToken.None);
        var list = await learnerSvc.ListAsync(new ListKanjiRequest(), CancellationToken.None);

        Assert.Null(detail);
        Assert.Empty(list.Items);
    }
}