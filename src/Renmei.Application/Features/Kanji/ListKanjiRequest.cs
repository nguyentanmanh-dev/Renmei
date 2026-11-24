using System.ComponentModel.DataAnnotations;

namespace Renmei.Application.Features.Kanji;

public sealed class ListKanjiRequest
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    [Range(1, 5)]
    public int? Level { get; init; }

    [StringLength(50)]
    public string? Q { get; init; }
}