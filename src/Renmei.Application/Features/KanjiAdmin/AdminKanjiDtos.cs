using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Renmei.Application.Features.Kanji;
using Renmei.Domain.Enums;

namespace Renmei.Application.Features.KanjiAdmin;

public enum KanjiError
{
    NotFound,
    Duplicate,
    NotEditable,
    PreconditionFailed,
    Invalid,
    SourceMissing
}

public sealed record KanjiResult<T>(T? Value, KanjiError? Error = null, string? Detail = null)
{
    public bool IsSuccess => Error is null;
}

public sealed class ListAdminKanjiRequest
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    [Range(1, 5)]
    public int? Level { get; init; }

    public KanjiStatus? Status { get; init; }

    [StringLength(50)]
    public string? Q { get; init; }
}

public sealed record SaveReadingRequest(
    ReadingType Type,
    [Required, StringLength(50)] string Kana,
    [StringLength(50)] string? Romaji);

public sealed record SaveMeaningRequest(
    [Required, StringLength(10)] string Language,
    MeaningType Type,
    [Required, StringLength(300)] string Text,
    int SortOrder = 0);

public sealed record SaveExampleWordRequest(
    [Required, StringLength(100)] string Word,
    [Required, StringLength(100)] string Reading,
    [Required, StringLength(300)] string MeaningVi,
    [StringLength(500)] string? SentenceJa);

public sealed record SaveKanjiRequest(
    [Required] string Character,
    [Range(1, 5)] int JlptLevel,
    int? StrokeCount,
    [StringLength(4)] string? Radical,
    [StringLength(500)] string? MnemonicVi,
    [StringLength(500)] string? MnemonicImageUrl,
    [StringLength(500)] string? StrokeOrderGifUrl,
    [StringLength(500)] string? AudioUrl,
    int? SourceId,
    IReadOnlyList<SaveReadingRequest>? Readings,
    IReadOnlyList<SaveMeaningRequest>? Meanings,
    IReadOnlyList<SaveExampleWordRequest>? ExampleWords);

public sealed record AdminKanjiSummaryDto(
    int Id,
    string Character,
    int JlptLevel,
    KanjiStatus Status,
    IReadOnlyList<string> HanViet,
    bool HasImage,
    bool HasStrokeAnimation,
    bool HasAudio);

public sealed record KanjiAdminDetailDto(
    int Id,
    string Character,
    int JlptLevel,
    KanjiStatus Status,
    int? StrokeCount,
    string? Radical,
    string? MnemonicVi,
    string? MnemonicImageUrl,
    string? StrokeOrderGifUrl,
    string? AudioUrl,
    int SourceId,
    IReadOnlyList<ReadingDto> Readings,
    IReadOnlyList<MeaningDto> Meanings,
    IReadOnlyList<ExampleWordDto> ExampleWords,
    [property: JsonIgnore] byte[] RowVersion);