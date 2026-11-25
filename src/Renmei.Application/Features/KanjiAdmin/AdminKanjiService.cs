using System.Text;
using Microsoft.EntityFrameworkCore;
using Renmei.Application.Abstractions;
using Renmei.Application.Common;
using Renmei.Application.Features.Kanji;
using Renmei.Domain.Content;
using Renmei.Domain.Enums;

namespace Renmei.Application.Features.KanjiAdmin;

public sealed class AdminKanjiService(IApplicationDbContext db, ICurrentUser currentUser) : IAdminKanjiService
{
    private const string DefaultSourceName = "Tự biên soạn";
    private const int MaxCollectionItems = 50;

    public async Task<PagedResult<AdminKanjiSummaryDto>> ListAsync(ListAdminKanjiRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = db.Kanji.AsNoTracking();

        if (request.Level is { } level)
        {
            query = query.Where(k => k.JlptLevel == level);
        }

        if (request.Status is { } status)
        {
            query = query.Where(k => k.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var term = NormalizeText(request.Q)!;
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
            .Select(k => new AdminKanjiSummaryDto(
                k.Id,
                k.Character,
                k.JlptLevel,
                k.Status,
                k.Meanings
                    .Where(m => m.Type == MeaningType.HanViet)
                    .OrderBy(m => m.SortOrder)
                    .ThenBy(m => m.Id)
                    .Select(m => m.Text)
                    .ToList(),
                k.MnemonicImageUrl != null && k.MnemonicImageUrl != "",
                k.StrokeOrderGifUrl != null && k.StrokeOrderGifUrl != "",
                k.AudioUrl != null && k.AudioUrl != ""))
            .ToListAsync(ct);

        return new PagedResult<AdminKanjiSummaryDto>(items, request.Page, request.PageSize, total);
    }

    public Task<KanjiAdminDetailDto?> GetDetailAsync(int id, CancellationToken ct)
        => db.Kanji.AsNoTracking()
            .Where(k => k.Id == id)
            .Select(k => new KanjiAdminDetailDto(
                k.Id,
                k.Character,
                k.JlptLevel,
                k.Status,
                k.StrokeCount,
                k.Radical,
                k.MnemonicVi,
                k.MnemonicImageUrl,
                k.StrokeOrderGifUrl,
                k.AudioUrl,
                k.SourceId,
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
                    .ToList(),
                k.RowVersion))
            .FirstOrDefaultAsync(ct);

    public async Task<KanjiResult<KanjiAdminDetailDto>> CreateAsync(SaveKanjiRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationError = ValidateRequest(request);
        if (validationError is not null)
        {
            return new(default, KanjiError.Invalid, validationError);
        }

        var sourceId = await ResolveSourceIdAsync(request.SourceId, ct);
        if (sourceId is null)
        {
            return new(default, KanjiError.SourceMissing, $"Content source '{DefaultSourceName}' or requested sourceId does not exist.");
        }

        var normalizedChar = NormalizeText(request.Character)!;
        if (await db.Kanji.AnyAsync(k => k.Character == normalizedChar, ct))
        {
            return new(default, KanjiError.Duplicate, $"Kanji '{normalizedChar}' already exists.");
        }

        var kanji = new Domain.Content.Kanji(normalizedChar, request.JlptLevel, sourceId.Value, currentUser.UserId, DateTime.UtcNow);
        kanji.UpdateContent(
            request.JlptLevel,
            request.StrokeCount,
            NormalizeText(request.Radical),
            NormalizeText(request.MnemonicVi),
            NormalizeText(request.MnemonicImageUrl),
            NormalizeText(request.StrokeOrderGifUrl),
            NormalizeText(request.AudioUrl),
            sourceId.Value,
            currentUser.UserId);

        ApplyChildren(kanji, request, sourceId.Value);
        db.Kanji.Add(kanji);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return new(default, KanjiError.Duplicate, $"Kanji '{normalizedChar}' already exists.");
        }

        return new(await GetDetailAsync(kanji.Id, ct));
    }

    public async Task<KanjiResult<KanjiAdminDetailDto>> UpdateAsync(
        int id,
        byte[] expectedVersion,
        SaveKanjiRequest request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var k = await db.Kanji
            .Include(x => x.Readings)
            .Include(x => x.Meanings)
            .Include(x => x.ExampleWords)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (k is null)
        {
            return new(default, KanjiError.NotFound);
        }

        if (!k.RowVersion.AsSpan().SequenceEqual(expectedVersion))
        {
            return new(default, KanjiError.PreconditionFailed, "RowVersion mismatch.");
        }

        if (!k.IsEditable)
        {
            return new(default, KanjiError.NotEditable, $"Kanji in status {k.Status} cannot be edited.");
        }

        var normalizedChar = NormalizeText(request.Character);
        if (!string.Equals(normalizedChar, k.Character, StringComparison.Ordinal))
        {
            return new(default, KanjiError.Invalid, "Character cannot be changed.");
        }

        var validationError = ValidateRequest(request);
        if (validationError is not null)
        {
            return new(default, KanjiError.Invalid, validationError);
        }

        var sourceId = await ResolveSourceIdAsync(request.SourceId ?? k.SourceId, ct);
        if (sourceId is null)
        {
            return new(default, KanjiError.SourceMissing, "Content source does not exist.");
        }

        db.SetOriginalRowVersion(k, expectedVersion);

        k.UpdateContent(
            request.JlptLevel,
            request.StrokeCount,
            NormalizeText(request.Radical),
            NormalizeText(request.MnemonicVi),
            NormalizeText(request.MnemonicImageUrl),
            NormalizeText(request.StrokeOrderGifUrl),
            NormalizeText(request.AudioUrl),
            sourceId.Value,
            currentUser.UserId);

        ApplyChildren(k, request, sourceId.Value);
        db.MarkAsModified(k);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(default, KanjiError.PreconditionFailed, "Concurrent modification detected.");
        }

        return new(await GetDetailAsync(id, ct));
    }

    public async Task<KanjiResult<KanjiAdminDetailDto>> ArchiveAsync(int id, byte[] expectedVersion, CancellationToken ct)
    {
        var k = await db.Kanji.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (k is null)
        {
            return new(default, KanjiError.NotFound);
        }

        if (!k.RowVersion.AsSpan().SequenceEqual(expectedVersion))
        {
            return new(default, KanjiError.PreconditionFailed, "RowVersion mismatch.");
        }

        db.SetOriginalRowVersion(k, expectedVersion);
        k.Archive(currentUser.UserId);
        db.MarkAsModified(k);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(default, KanjiError.PreconditionFailed, "Concurrent modification detected.");
        }

        return new(await GetDetailAsync(id, ct));
    }

    private async Task<int?> ResolveSourceIdAsync(int? requestedSourceId, CancellationToken ct)
    {
        if (requestedSourceId is { } id)
        {
            var exists = await db.ContentSources.AnyAsync(s => s.Id == id, ct);
            return exists ? id : null;
        }

        return await db.ContentSources
            .Where(s => s.Name == DefaultSourceName)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(ct);
    }

    private static void ApplyChildren(Domain.Content.Kanji k, SaveKanjiRequest request, int sourceId)
    {
        var readings = (request.Readings ?? [])
            .Select(r => new KanjiReading(k.Id, r.Type, NormalizeText(r.Kana)!, NormalizeText(r.Romaji)));
        k.ReplaceReadings(readings);

        var meanings = (request.Meanings ?? [])
            .Select(m => new KanjiMeaning(
                k.Id,
                m.Type,
                NormalizeText(m.Text)!,
                isPrimary: m.SortOrder == 0,
                sortOrder: m.SortOrder));
        k.ReplaceMeanings(meanings);

        var examples = (request.ExampleWords ?? [])
            .Select((e, idx) => new ExampleWord(
                k.Id,
                NormalizeText(e.Word)!,
                NormalizeText(e.Reading)!,
                NormalizeText(e.MeaningVi)!,
                sourceId,
                sortOrder: idx));
        k.ReplaceExampleWords(examples);
    }

    private static string? ValidateRequest(SaveKanjiRequest r)
    {
        var ch = NormalizeText(r.Character);
        if (string.IsNullOrEmpty(ch) || ch.Any(char.IsWhiteSpace) || ch.EnumerateRunes().Count() != 1)
        {
            return "Character must be a single non-whitespace Unicode rune.";
        }

        if (r.JlptLevel is < 1 or > 5)
        {
            return "JlptLevel must be between 1 and 5.";
        }

        if (r.StrokeCount is <= 0)
        {
            return "StrokeCount must be greater than 0.";
        }

        if (r.Radical is { Length: > 4 } || r.MnemonicVi is { Length: > 500 })
        {
            return "Radical or MnemonicVi exceeds maximum length.";
        }

        if (!IsSafeMediaUrl(r.MnemonicImageUrl) || !IsSafeMediaUrl(r.StrokeOrderGifUrl) || !IsSafeMediaUrl(r.AudioUrl))
        {
            return "Media URLs must be relative paths or https URLs up to 500 characters.";
        }

        if ((r.Readings?.Count ?? 0) > MaxCollectionItems
            || (r.Meanings?.Count ?? 0) > MaxCollectionItems
            || (r.ExampleWords?.Count ?? 0) > MaxCollectionItems)
        {
            return $"Child collections cannot exceed {MaxCollectionItems} items.";
        }

        if (r.Readings is not null && r.Readings.Any(x => !Enum.IsDefined(x.Type) || string.IsNullOrWhiteSpace(x.Kana) || x.Kana.Trim().Length > 50))
        {
            return "Each reading requires valid Type (On/Kun) and non-empty Kana up to 50 characters.";
        }

        if (r.Meanings is not null)
        {
            foreach (var m in r.Meanings)
            {
                var lang = m.Language?.Trim();
                if (!Enum.IsDefined(m.Type) || lang is not ("vi" or "en") || string.IsNullOrWhiteSpace(m.Text) || m.Text.Trim().Length > 300)
                {
                    return "Each meaning requires Language ('vi' or 'en'), valid Type, and non-empty Text up to 300 characters.";
                }
            }
        }

        if (r.ExampleWords is not null && r.ExampleWords.Any(e =>
            string.IsNullOrWhiteSpace(e.Word) || e.Word.Trim().Length > 100
            || string.IsNullOrWhiteSpace(e.Reading) || e.Reading.Trim().Length > 100
            || string.IsNullOrWhiteSpace(e.MeaningVi) || e.MeaningVi.Trim().Length > 300))
        {
            return "Each example word requires non-empty Word (<=100), Reading (<=100), and MeaningVi (<=300).";
        }

        return null;
    }

    private static bool IsSafeMediaUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return true;
        }

        var trimmed = url.Trim();
        if (trimmed.Length > 500)
        {
            return false;
        }

        if (trimmed.StartsWith('/') && !trimmed.StartsWith("//", StringComparison.Ordinal))
        {
            return true;
        }

        return Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
               && string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizeText(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().Normalize(NormalizationForm.FormC);

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("2601", StringComparison.Ordinal) == true
           || ex.InnerException?.Message.Contains("2627", StringComparison.Ordinal) == true
           || ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true;
}