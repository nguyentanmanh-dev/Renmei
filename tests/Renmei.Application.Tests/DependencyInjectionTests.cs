using Microsoft.Extensions.DependencyInjection;
using Renmei.Application;

namespace Renmei.Application.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_ShouldReturnSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        Assert.Same(services, result);
    }
}
