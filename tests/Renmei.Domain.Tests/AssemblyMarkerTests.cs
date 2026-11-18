using Renmei.Domain;

namespace Renmei.Domain.Tests;

public sealed class AssemblyMarkerTests
{
    [Fact]
    public void AssemblyMarker_ShouldBelongTo_RenmeiDomainAssembly()
    {
        var assemblyName = typeof(AssemblyMarker).Assembly.GetName().Name;
        Assert.Equal("Renmei.Domain", assemblyName);
    }
}