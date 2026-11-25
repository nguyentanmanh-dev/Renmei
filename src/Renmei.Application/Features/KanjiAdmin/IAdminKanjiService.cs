using Renmei.Application.Common;

namespace Renmei.Application.Features.KanjiAdmin;

public interface IAdminKanjiService
{
    Task<PagedResult<AdminKanjiSummaryDto>> ListAsync(ListAdminKanjiRequest request, CancellationToken ct);
    Task<KanjiAdminDetailDto?> GetDetailAsync(int id, CancellationToken ct);
    Task<KanjiResult<KanjiAdminDetailDto>> CreateAsync(SaveKanjiRequest request, CancellationToken ct);
    Task<KanjiResult<KanjiAdminDetailDto>> UpdateAsync(int id, byte[] expectedVersion, SaveKanjiRequest request, CancellationToken ct);
    Task<KanjiResult<KanjiAdminDetailDto>> ArchiveAsync(int id, byte[] expectedVersion, CancellationToken ct);
}