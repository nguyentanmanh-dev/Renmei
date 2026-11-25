using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using Renmei.WebApi.Common;
using Renmei.WebApi.Controllers;

namespace Renmei.Architecture.Tests;

public sealed class EndpointSecurityTests
{
    [Fact(Skip = "Bật ở tuần 7 khi có JWT")]
    public void KanjiController_requires_authentication()
    {
        var hasAuthorize = typeof(KanjiController).IsDefined(typeof(AuthorizeAttribute), inherit: true);
        Assert.True(hasAuthorize, "Thêm [Authorize] cho KanjiController ở tuần 7.");
    }

    [Fact(Skip = "Bật ở tuần 7 khi có JWT")]
    public void AdminKanjiController_requires_authentication()
    {
        var hasAuthorize = typeof(AdminKanjiController).IsDefined(typeof(AuthorizeAttribute), inherit: true);
        Assert.True(hasAuthorize, "Gỡ [DevelopmentOnly] và gắn [Authorize(Policy = \"EditorOrAdmin\")] ở tuần 7.");
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("*", false)]
    [InlineData("W/\"AQID\"", false)]
    [InlineData("AQID", false)]
    [InlineData("\"not-base64!\"", false)]
    [InlineData("\"\"", false)]
    [InlineData("\"AQIDBAUGBwg=\"", true)]
    public void ETagHelper_TryParseIfMatch_validates_strong_etags(string? raw, bool expected)
    {
        var values = raw is null ? StringValues.Empty : new StringValues(raw);
        var ok = ETagHelper.TryParseIfMatch(values, out var bytes);

        Assert.Equal(expected, ok);
        if (expected)
        {
            Assert.Equal(8, bytes.Length);
        }
    }

    [Fact]
    public void ETagHelper_TryParseIfMatch_rejects_multiple_header_values()
    {
        var values = new StringValues(["\"AQIDBAUGBwg=\"", "\"CAcGBQQDAgE=\""]);
        Assert.False(ETagHelper.TryParseIfMatch(values, out _));
    }
}