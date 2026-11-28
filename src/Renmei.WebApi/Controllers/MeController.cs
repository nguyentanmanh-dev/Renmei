using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Renmei.Application.Abstractions;
using Renmei.Application.Features.Auth;

namespace Renmei.WebApi.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public sealed class MeController(ICurrentUser currentUser, IApplicationDbContext db) : ControllerBase
{
    [HttpGet(Name = "getMe")]
    [ProducesResponseType<UserProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> GetMe(CancellationToken ct)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Unauthorized();
        }

        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            return Unauthorized();
        }

        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
        var roles = await db.Roles
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id))
            .OrderBy(r => r.Id)
            .Select(r => r.Name)
            .ToListAsync(ct);

        return Ok(new UserProfileDto(user.Id, user.Email, user.DisplayName, roles));
    }
}