using Microsoft.Extensions.Primitives;

namespace Renmei.WebApi.Common;

public static class ETagHelper
{
    public static string ToETag(byte[] version) => $"\"{Convert.ToBase64String(version)}\"";

    public static bool TryParseIfMatch(StringValues header, out byte[] version)
    {
        version = [];
        if (header.Count != 1)
        {
            return false;
        }

        var raw = header[0]?.Trim();
        if (string.IsNullOrEmpty(raw) || raw == "*" || raw.StartsWith("W/", StringComparison.Ordinal))
        {
            return false;
        }

        if (raw.Length < 3 || raw[0] != '"' || raw[^1] != '"')
        {
            return false;
        }

        try
        {
            version = Convert.FromBase64String(raw[1..^1]);
            return version.Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}