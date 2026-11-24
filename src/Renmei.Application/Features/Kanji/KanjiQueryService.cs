using Microsoft.EntityFrameworkCore;
using Renmei.Application.Abstractions;
using Renmei.Application.Common;
using Renmei.Domain.Enums;

namespace Renmei.Application.Features.Kanji;

internal static class KanjiQueryExtensions
{
    public static IQueryable<Domain.Content.Kanji> Published(this IQueryable<Domain.Content.Kanji> q)
        => q.Where(k => k.Status == KanjiStatus.Published);
}

public sealed class KanjiQueryService(IApplicationDbContext db) : IKanjiQueryService
{
    public async Task<PagedResult<KanjiSummaryDto>> ListAsync(ListKanjiRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = db.Kanji.AsNoTracking().Published();

        if (request.Level is { } level)
        {
            query = query.Where(k => k.JlptLevel == level);
        }

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var term = request.Q.Trim();
            query = term.EnumerateRunes().Count() == 1
                ? query.Where(k => k.Character == term)
                : query.Where(k => k.Meanings.Any(m => m.Text.Contains(term)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(k => k.JlptLevel)
            .ThenBy(k => k.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(k => new KanjiSummaryDto(
                k.Id,
                k.Character,
                k.JlptLevel,
                k.Meanings
                    .Where(m => m.Type == MeaningType.HanViet)
                    .OrderBy(m => m.SortOrder)
                    .ThenBy(m => m.Id)
                    .Select(m => m.Text)
                    .ToList()))
            .ToListAsync(ct);

        return new PagedResult<KanjiSummaryDto>(items, request.Page, request.PageSize, total);
    }

    public Task<KanjiDetailDto?> GetAsync(int id, CancellationToken ct)
        => db.Kanji.AsNoTracking().Published()
            .Where(k => k.Id == id)
            .Select(k => new KanjiDetailDto(
                k.Id,
                k.Character,
                k.JlptLevel,
                k.StrokeCount,
                k.Radical,
                k.MnemonicVi,
                k.MnemonicImageUrl,
                k.StrokeOrderGifUrl,
                k.AudioUrl,
                k.Readings
                    .OrderBy(x => x.Id)
                    .Select(x => new ReadingDto(x.Type, x.Reading, x.Romaji))
                    .ToList(),
                k.Meanings
                    .OrderBy(m => m.SortOrder)
                    .ThenBy(m => m.Id)
                    .Select(m => new MeaningDto("vi", m.Type, m.Text))
                    .ToList(),
                k.ExampleWords
                    .OrderBy(e => e.SortOrder)
                    .ThenBy(e => e.Id)
                    .Select(e => new ExampleWordDto(e.Word, e.Reading, e.MeaningVi, null))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
}