using Microsoft.AspNetCore.Mvc;
using Renmei.Application.Common;
using Renmei.Application.Features.Kanji;

namespace Renmei.WebApi.Controllers;

[ApiController]
[Route("api/kanji")]
public sealed class KanjiController(IKanjiQueryService service) : ControllerBase
{
    [HttpGet(Name = "listKanji")]
    [ProducesResponseType<PagedResult<KanjiSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<KanjiSummaryDto>>> List(
        [FromQuery] ListKanjiRequest request,
        CancellationToken ct)
        => Ok(await service.ListAsync(request, ct));

    [HttpGet("{id:int}", Name = "getKanji")]
    [ProducesResponseType<KanjiDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KanjiDetailDto>> Get(int id, CancellationToken ct)
    {
        var dto = await service.GetAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}