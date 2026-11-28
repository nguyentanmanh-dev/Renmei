using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Renmei.Application.Features.Auth;

public static class RefreshTokenSecrets
{
    public static (string Raw, string Hash) Generate()
    {
        var raw = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));
        return (raw, Hash(raw));
    }

    public static string Hash(string raw)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw))).ToLowerInvariant();
}