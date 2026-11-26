using Renmei.Domain.Content;
using Renmei.Domain.Enums;

namespace Renmei.Application.Tests.Support;

internal static class EntityExtensions
{
    public static void SetStatus(this Kanji k, KanjiStatus status)
        => typeof(Kanji).GetProperty(nameof(Kanji.Status))!.SetValue(k, status);

    public static void SetRowVersion(this Kanji k, byte[] version)
        => typeof(Kanji).GetProperty(nameof(Kanji.RowVersion))!.SetValue(k, version);
}