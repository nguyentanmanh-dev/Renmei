using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Renmei.Application.Features.Auth;
using Renmei.Infrastructure.Auth;

namespace Renmei.Infrastructure.Tests.Auth;

public sealed class JwtAndSecretsTests
{
    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private static readonly string ValidBase64Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    private static readonly DateTimeOffset FixedNow = new(2026, 11, 28, 10, 0, 0, TimeSpan.Zero);

    private static JwtAccessTokenGenerator CreateGenerator(string? key = null, int minutes = 15)
        => new(
            Options.Create(new JwtOptions { Issuer = "renmei-api", Audience = "renmei-clients", SigningKey = key ?? ValidBase64Key }),
            Options.Create(new TokenOptions { AccessTokenMinutes = minutes }),
            new FixedTimeProvider(FixedNow));

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public void RefreshTokenSecrets_generates_1000_unique_43_char_tokens_and_deterministic_64_char_hashes()
    {
        var generated = Enumerable.Range(0, 1000).Select(_ => RefreshTokenSecrets.Generate()).ToList();

        Assert.Equal(1000, generated.Select(x => x.Raw).Distinct().Count());
        Assert.All(generated, item =>
        {
            Assert.Equal(43, item.Raw.Length);
            Assert.Equal(64, item.Hash.Length);
            Assert.NotEqual(item.Raw, item.Hash);
            Assert.Equal(item.Hash, RefreshTokenSecrets.Hash(item.Raw));
        });
    }

    [Fact]
    [Trait("Requirement", "FR-001")]
    public async Task JwtAccessTokenGenerator_produces_valid_token_with_expected_claims_and_no_pii()
    {
        var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var sut = CreateGenerator(minutes: 15);

        var result = sut.Generate(userId, ["User", "Editor"]);

        Assert.Equal(FixedNow.UtcDateTime.AddMinutes(15), result.ExpiresAtUtc);
        Assert.Equal(900, result.ExpiresInSeconds);

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "renmei-api",
            ValidateAudience = true,
            ValidAudience = "renmei-clients",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(ValidBase64Key)),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ValidateLifetime = false
        };

        var handler = new JsonWebTokenHandler();
        var validation = await handler.ValidateTokenAsync(result.Token, validationParams);

        Assert.True(validation.IsValid);
        var jwt = handler.ReadJsonWebToken(result.Token);
        Assert.Equal(userId.ToString(), jwt.Subject);
        Assert.False(string.IsNullOrWhiteSpace(jwt.Id));
        Assert.Equal(["User", "Editor"], jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value));
        Assert.DoesNotContain(jwt.Claims, c => c.Type is "email" or "name" or "unique_name");
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public async Task JwtAccessTokenGenerator_token_fails_with_wrong_key_or_tampered_payload()
    {
        var generated = CreateGenerator().Generate(Guid.NewGuid(), ["User"]);
        var otherKey = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(48));

        var handler = new JsonWebTokenHandler();
        var wrongKeyResult = await handler.ValidateTokenAsync(generated.Token, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = otherKey
        });
        Assert.False(wrongKeyResult.IsValid);

        var parts = generated.Token.Split('.');
        var tamperedChar = parts[1][0] == 'a' ? 'b' : 'a';
        var tamperedToken = $"{parts[0]}.{tamperedChar}{parts[1][1..]}.{parts[2]}";

        var tamperedResult = await handler.ValidateTokenAsync(tamperedToken, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(ValidBase64Key))
        });
        Assert.False(tamperedResult.IsValid);
    }
}