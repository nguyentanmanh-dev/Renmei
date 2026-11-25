using Microsoft.AspNetCore.Mvc;
using Renmei.Application.Common;
using Renmei.Application.Features.KanjiAdmin;
using Renmei.WebApi.Common;
using Renmei.WebApi.Filters;

namespace Renmei.WebApi.Controllers;

[ApiController]
[Route("api/admin/kanji")]
[DevelopmentOnly]
public sealed class AdminKanjiController(IAdminKanjiService service) : ControllerBase
{
    [HttpGet(Name = "adminListKanji")]
    [ProducesResponseType<PagedResult<AdminKanjiSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminKanjiSummaryDto>>> List(
        [FromQuery] ListAdminKanjiRequest request,
        CancellationToken ct)
        => Ok(await service.ListAsync(request, ct));

    [HttpGet("{id:int}", Name = "adminGetKanji")]
    [ProducesResponseType<KanjiAdminDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KanjiAdminDetailDto>> Get(int id, CancellationToken ct)
    {
        var dto = await service.GetDetailAsync(id, ct);
        if (dto is null)
        {
            return NotFound();
        }

        Response.Headers.ETag = ETagHelper.ToETag(dto.RowVersion);
        return Ok(dto);
    }

    [HttpPost(Name = "adminCreateKanji")]
    [ProducesResponseType<KanjiAdminDetailDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<KanjiAdminDetailDto>> Post(SaveKanjiRequest body, CancellationToken ct)
    {
        var res = await service.CreateAsync(body, ct);
        if (!res.IsSuccess)
        {
            return MapError(res);
        }

        Response.Headers.ETag = ETagHelper.ToETag(res.Value!.RowVersion);
        return CreatedAtRoute("adminGetKanji", new { id = res.Value.Id }, res.Value);
    }

    [HttpPut("{id:int}", Name = "adminUpdateKanji")]
    [ProducesResponseType<KanjiAdminDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<ActionResult<KanjiAdminDetailDto>> Put(int id, SaveKanjiRequest body, CancellationToken ct)
    {
        if (!Request.Headers.ContainsKey("If-Match"))
        {
            return Problem(statusCode: 428, type: "precondition-required", title: "If-Match header is required.");
        }

        if (!ETagHelper.TryParseIfMatch(Request.Headers.IfMatch, out var version))
        {
            return Problem(statusCode: 400, type: "invalid-etag", title: "If-Match must be a single strong ETag.");
        }

        var res = await service.UpdateAsync(id, version, body, ct);
        if (!res.IsSuccess)
        {
            return MapError(res);
        }

        Response.Headers.ETag = ETagHelper.ToETag(res.Value!.RowVersion);
        return Ok(res.Value);
    }

    [HttpPost("{id:int}/archive", Name = "adminArchiveKanji")]
    [ProducesResponseType<KanjiAdminDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<ActionResult<KanjiAdminDetailDto>> Archive(int id, CancellationToken ct)
    {
        if (!Request.Headers.ContainsKey("If-Match"))
        {
            return Problem(statusCode: 428, type: "precondition-required", title: "If-Match header is required.");
        }

        if (!ETagHelper.TryParseIfMatch(Request.Headers.IfMatch, out var version))
        {
            return Problem(statusCode: 400, type: "invalid-etag", title: "If-Match must be a single strong ETag.");
        }

        var res = await service.ArchiveAsync(id, version, ct);
        if (!res.IsSuccess)
        {
            return MapError(res);
        }

        Response.Headers.ETag = ETagHelper.ToETag(res.Value!.RowVersion);
        return Ok(res.Value);
    }

    private ActionResult MapError<T>(KanjiResult<T> res) => res.Error switch
    {
        KanjiError.NotFound => Problem(statusCode: 404, type: "kanji-not-found", title: "Kanji not found.", detail: res.Detail),
        KanjiError.Duplicate => Problem(statusCode: 409, type: "kanji-duplicate", title: "Kanji already exists.", detail: res.Detail),
        KanjiError.NotEditable => Problem(statusCode: 409, type: "kanji-not-editable", title: "Kanji cannot be edited in its current status.", detail: res.Detail),
        KanjiError.PreconditionFailed => Problem(statusCode: 412, type: "precondition-failed", title: "ETag version mismatch.", detail: res.Detail),
        KanjiError.SourceMissing => Problem(statusCode: 400, type: "content-source-missing", title: "Default or specified content source is missing.", detail: res.Detail),
        _ => Problem(statusCode: 400, type: "validation-error", title: "Invalid kanji request.", detail: res.Detail)
    };
}