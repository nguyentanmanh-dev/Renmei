using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Renmei.Application.Features.Auth;

namespace Renmei.Infrastructure.Auth;

public sealed class JwtAccessTokenGenerator(
    IOptions<JwtOptions> jwtOptions,
    IOptions<TokenOptions> tokenOptions,
    TimeProvider clock) : IAccessTokenGenerator
{
    private readonly SymmetricSecurityKey _key = new(Convert.FromBase64String(jwtOptions.Value.SigningKey));
    private readonly JsonWebTokenHandler _handler = new();

    public AccessTokenResult Generate(Guid userId, IReadOnlyCollection<string> roles)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var expiresInSeconds = tokenOptions.Value.AccessTokenMinutes * 60;
        var expires = now.AddSeconds(expiresInSeconds);

        var claims = new List<Claim>
        {
            new("sub", userId.ToString()),
            new("jti", Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Value.Issuer,
            Audience = jwtOptions.Value.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = expires,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
        };

        return new AccessTokenResult(_handler.CreateToken(descriptor), expires, expiresInSeconds);
    }
}