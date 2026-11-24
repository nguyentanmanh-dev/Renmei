using Microsoft.AspNetCore.Authorization;
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
}