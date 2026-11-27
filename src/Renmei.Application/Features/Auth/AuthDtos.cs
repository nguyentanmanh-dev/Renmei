using System.ComponentModel.DataAnnotations;

namespace Renmei.Application.Features.Auth;

public enum AuthError
{
    InvalidInput,
    EmailTaken,
    InvalidCredentials,
    AccountLocked
}

public sealed record AuthResult<T>(T? Value, AuthError? Error = null, string? Detail = null)
{
    public bool IsSuccess => Error is null;
}

public sealed record RegisterCommand(string Email, string Password, string? DisplayName);

public sealed record RegisterRequest(
    [Required, StringLength(256)] string Email,
    [Required, StringLength(128, MinimumLength = 8)] string Password,
    [StringLength(100)] string? DisplayName);

public sealed record LoginRequest(
    [Required, StringLength(256)] string Email,
    [Required, StringLength(128)] string Password);

public sealed record TokenPairDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);