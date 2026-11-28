using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renmei.Application.Features.Auth;

namespace Renmei.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService auth, ITokenService tokens) : ControllerBase
{
    [HttpPost("register", Name = "register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest r, CancellationToken ct)
    {
        var res = await auth.RegisterAsync(new RegisterCommand(r.Email, r.Password, r.DisplayName), ct);
        return res.Error switch
        {
            null => Created(string.Empty, new { id = res.Value, status = "Created" }),
            AuthError.EmailTaken => Problem(statusCode: 409, title: "Email is already registered.", type: "auth-email-taken"),
            _ => Problem(statusCode: 400, title: "Invalid registration data.", type: "auth-invalid-input")
        };
    }

    [HttpPost("login", Name = "login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login(LoginRequest r, CancellationToken ct)
    {
        var res = await auth.AuthenticateAsync(r.Email, r.Password, ct);
        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                AuthError.AccountLocked => Problem(statusCode: 403, title: "Account is not active.", type: "auth-account-locked"),
                _ => Problem(statusCode: 401, title: "Invalid email or password.", type: "auth-invalid-credentials")
            };
        }

        return Ok(await tokens.IssueAsync(res.Value!, ct));
    }

    [HttpPost("refresh", Name = "refreshToken")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(RefreshRequest r, CancellationToken ct)
    {
        var res = await tokens.RefreshAsync(r.RefreshToken, ct);
        return res.IsSuccess
            ? Ok(res.Value)
            : Problem(statusCode: 401, title: "Invalid refresh token.", type: "auth-invalid-refresh");
    }

    [HttpPost("logout", Name = "logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(RefreshRequest r, CancellationToken ct)
    {
        await tokens.RevokeAsync(r.RefreshToken, ct);
        return NoContent();
    }
}