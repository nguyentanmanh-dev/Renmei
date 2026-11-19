using System.Reflection;

public class ArchitectureTests
{
    static readonly Assembly Domain = typeof(Renmei.Domain.AssemblyMarker).Assembly;
    static readonly Assembly Application = typeof(Renmei.Application.DependencyInjection).Assembly;
    static readonly Assembly Infrastructure = typeof(Renmei.Infrastructure.DependencyInjection).Assembly;

    static bool References(Assembly a, string name) =>
        a.GetReferencedAssemblies().Any(r => r.Name!.StartsWith(name));

    [Fact]
    public void Domain_references_nothing_of_ours_or_efcore()
    {
        Assert.False(References(Domain, "Renmei.Application"));
        Assert.False(References(Domain, "Renmei.Infrastructure"));
        Assert.False(References(Domain, "Renmei.WebApi"));
        Assert.False(References(Domain, "Microsoft.EntityFrameworkCore"));
    }

    [Fact]
    public void Application_does_not_reference_Infrastructure_or_WebApi_or_SqlServer()
    {
        var applicationAssembly = typeof(Renmei.Application.Abstractions.IApplicationDbContext).Assembly;
        var referencedAssemblies = applicationAssembly.GetReferencedAssemblies();

        var hasForbiddenReference = referencedAssemblies.Any(a =>
            a.Name is not null &&
            (a.Name.Equals("Renmei.Infrastructure", StringComparison.OrdinalIgnoreCase) ||
             a.Name.Equals("Renmei.WebApi", StringComparison.OrdinalIgnoreCase) ||
             a.Name.Equals("Microsoft.EntityFrameworkCore.SqlServer", StringComparison.OrdinalIgnoreCase)));

        Assert.False(hasForbiddenReference);
    }

    [Fact]
    public void Infrastructure_does_not_reference_WebApi()
        => Assert.False(References(Infrastructure, "Renmei.WebApi"));
}