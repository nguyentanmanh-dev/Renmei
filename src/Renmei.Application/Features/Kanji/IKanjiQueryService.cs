using Renmei.Application.Common;

namespace Renmei.Application.Features.Kanji;

public interface IKanjiQueryService
{
    Task<PagedResult<KanjiSummaryDto>> ListAsync(ListKanjiRequest request, CancellationToken ct);
    Task<KanjiDetailDto?> GetAsync(int id, CancellationToken ct);
}