using Renmei.Domain.Enums;

namespace Renmei.Application.Features.Kanji;

public sealed record KanjiSummaryDto(
    int Id,
    string Character,
    int JlptLevel,
    IReadOnlyList<string> HanViet);

public sealed record ReadingDto(
    ReadingType Type,
    string Kana,
    string? Romaji);

public sealed record MeaningDto(
    string Language,
    MeaningType Type,
    string Text);

public sealed record ExampleWordDto(
    string Word,
    string? Reading,
    string? MeaningVi,
    string? SentenceJa);

public sealed record KanjiDetailDto(
    int Id,
    string Character,
    int JlptLevel,
    int? StrokeCount,
    string? Radical,
    string? MnemonicVi,
    string? MnemonicImageUrl,
    string? StrokeOrderGifUrl,
    string? AudioUrl,
    IReadOnlyList<ReadingDto> Readings,
    IReadOnlyList<MeaningDto> Meanings,
    IReadOnlyList<ExampleWordDto> ExampleWords);