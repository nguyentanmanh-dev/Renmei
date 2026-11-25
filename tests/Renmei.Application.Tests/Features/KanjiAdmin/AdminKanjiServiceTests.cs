using Microsoft.EntityFrameworkCore;
using Renmei.Application.Abstractions;
using Renmei.Application.Features.Kanji;
using Renmei.Application.Features.KanjiAdmin;
using Renmei.Domain.Content;
using Renmei.Domain.Enums;
using Renmei.Infrastructure.Persistence;

namespace Renmei.Application.Tests.Features.KanjiAdmin;

public sealed class AdminKanjiServiceTests : IDisposable
{
    private sealed class StubCurrentUser : ICurrentUser
    {
        public Guid? UserId => Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    private readonly ApplicationDbContext _db;
    private readonly AdminKanjiService _sut;
    private readonly KanjiQueryService _learnerQuery;

    public AdminKanjiServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);
        _sut = new AdminKanjiService(_db, new StubCurrentUser());
        _learnerQuery = new KanjiQueryService(_db);
    }

    public void Dispose() => _db.Dispose();

    private async Task<ContentSource> SeedDefaultSourceAsync()
    {
        var src = new ContentSource("Tự biên soạn", "CC-BY-4.0", "Renmei Team", null);
        _db.ContentSources.Add(src);
        await _db.SaveChangesAsync();
        return src;
    }

    private static void SetRowVersion(Domain.Content.Kanji k, byte[] version)
        => typeof(Domain.Content.Kanji).GetProperty(nameof(Domain.Content.Kanji.RowVersion))!.SetValue(k, version);

    private static void SetStatus(Domain.Content.Kanji k, KanjiStatus status)
        => typeof(Domain.Content.Kanji).GetProperty(nameof(Domain.Content.Kanji.Status))!.SetValue(k, status);

    private static SaveKanjiRequest ValidRequest(string character = "学", string? imageUrl = null) => new(
        Character: character,
        JlptLevel: 5,
        StrokeCount: 8,
        Radical: "子",
        MnemonicVi: "Đứa trẻ dưới mái nhà đang học bài",
        MnemonicImageUrl: imageUrl,
        StrokeOrderGifUrl: null,
        AudioUrl: null,
        SourceId: null,
        Readings: [new SaveReadingRequest(ReadingType.On, "ガク", "gaku")],
        Meanings: [new SaveMeaningRequest("vi", MeaningType.HanViet, "học", 0)],
        ExampleWords: [new SaveExampleWordRequest("学生", "がくせい", "học sinh", null)]);

    [Fact]
    public async Task CreateAsync_creates_draft_with_default_source_and_children()
    {
        var src = await SeedDefaultSourceAsync();

        var res = await _sut.CreateAsync(ValidRequest(), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Value);
        Assert.Equal(KanjiStatus.Draft, res.Value.Status);
        Assert.Equal(src.Id, res.Value.SourceId);
        Assert.Single(res.Value.Readings);
        Assert.Single(res.Value.Meanings);
        Assert.Single(res.Value.ExampleWords);
    }

    [Fact]
    public async Task CreateAsync_returns_SourceMissing_when_default_source_absent()
    {
        var res = await _sut.CreateAsync(ValidRequest(), CancellationToken.None);
        Assert.Equal(KanjiError.SourceMissing, res.Error);
    }

    [Fact]
    public async Task CreateAsync_returns_Duplicate_when_character_exists()
    {
        await SeedDefaultSourceAsync();
        await _sut.CreateAsync(ValidRequest("学"), CancellationToken.None);

        var dup = await _sut.CreateAsync(ValidRequest("学"), CancellationToken.None);
        Assert.Equal(KanjiError.Duplicate, dup.Error);
    }

    [Theory]
    [InlineData("学生", null)]
    [InlineData("学", "javascript:alert(1)")]
    [InlineData("学", "data:text/html;base64,PHNjcmlwdD4=")]
    public async Task CreateAsync_returns_Invalid_for_bad_character_or_unsafe_url(string ch, string? url)
    {
        await SeedDefaultSourceAsync();
        var res = await _sut.CreateAsync(ValidRequest(ch, url), CancellationToken.None);
        Assert.Equal(KanjiError.Invalid, res.Error);
    }

    [Fact]
    public async Task UpdateAsync_replaces_children_on_matching_version_and_rejects_mismatch()
    {
        await SeedDefaultSourceAsync();
        var created = await _sut.CreateAsync(ValidRequest("学"), CancellationToken.None);
        var entity = await _db.Kanji.FirstAsync(x => x.Id == created.Value!.Id);
        byte[] v1 = [1, 2, 3, 4, 5, 6, 7, 8];
        SetRowVersion(entity, v1);
        await _db.SaveChangesAsync();

        var wrong = await _sut.UpdateAsync(entity.Id, [9, 9, 9, 9, 9, 9, 9, 9], ValidRequest("学"), CancellationToken.None);
        Assert.Equal(KanjiError.PreconditionFailed, wrong.Error);

        var updateReq = ValidRequest("学") with
        {
            ExampleWords = [new SaveExampleWordRequest("学校", "がっこう", "trường học", null)]
        };
        var ok = await _sut.UpdateAsync(entity.Id, v1, updateReq, CancellationToken.None);
        Assert.True(ok.IsSuccess);
        Assert.Single(ok.Value!.ExampleWords);
        Assert.Equal("学校", ok.Value.ExampleWords[0].Word);
    }

    [Fact]
    public async Task UpdateAsync_rejects_published_kanji_and_character_change()
    {
        await SeedDefaultSourceAsync();
        var created = await _sut.CreateAsync(ValidRequest("学"), CancellationToken.None);
        var entity = await _db.Kanji.FirstAsync(x => x.Id == created.Value!.Id);
        byte[] v1 = [1, 2, 3, 4, 5, 6, 7, 8];
        SetRowVersion(entity, v1);
        await _db.SaveChangesAsync();

        var changeChar = await _sut.UpdateAsync(entity.Id, v1, ValidRequest("校"), CancellationToken.None);
        Assert.Equal(KanjiError.Invalid, changeChar.Error);

        SetStatus(entity, KanjiStatus.Published);
        await _db.SaveChangesAsync();

        var editPub = await _sut.UpdateAsync(entity.Id, v1, ValidRequest("学"), CancellationToken.None);
        Assert.Equal(KanjiError.NotEditable, editPub.Error);
    }

    [Fact]
    public async Task ListAsync_returns_all_statuses_and_ArchiveAsync_hides_from_learner_api()
    {
        var src = await SeedDefaultSourceAsync();
        foreach (var (ch, st) in new[]
        {
            ("日", KanjiStatus.Draft),
            ("月", KanjiStatus.InReview),
            ("火", KanjiStatus.Published),
            ("水", KanjiStatus.Archived)
        })
        {
            var k = new Domain.Content.Kanji(ch, 5, src.Id, null, DateTime.UtcNow);
            SetStatus(k, st);
            SetRowVersion(k, [1, 1, 1, 1, 1, 1, 1, 1]);
            _db.Kanji.Add(k);
        }

        await _db.SaveChangesAsync();

        var all = await _sut.ListAsync(new ListAdminKanjiRequest(), CancellationToken.None);
        var onlyDraft = await _sut.ListAsync(new ListAdminKanjiRequest { Status = KanjiStatus.Draft }, CancellationToken.None);
        Assert.Equal(4, all.TotalItems);
        Assert.Single(onlyDraft.Items);

        var pub = await _db.Kanji.FirstAsync(x => x.Character == "火");
        var archived = await _sut.ArchiveAsync(pub.Id, pub.RowVersion, CancellationToken.None);
        Assert.True(archived.IsSuccess);
        Assert.Equal(KanjiStatus.Archived, archived.Value!.Status);

        var learnerDetail = await _learnerQuery.GetAsync(pub.Id, CancellationToken.None);
        Assert.Null(learnerDetail);
    }
}