using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renmei.Application.Features.Auth;

namespace Renmei.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(IAuthService auth, ITokenService tokens) : ControllerBase
{
    [HttpPost("register", Name = "register")]
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
    [ProducesResponseType<TokenPairDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
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

        try
        {
            var pair = await tokens.IssueAsync(res.Value!, ct);
            return Ok(pair);
        }
        catch (NotImplementedException)
        {
            return Problem(
                statusCode: StatusCodes.Status501NotImplemented,
                title: "JWT token issuance is not implemented yet.",
                type: "not-implemented");
        }
    }
}