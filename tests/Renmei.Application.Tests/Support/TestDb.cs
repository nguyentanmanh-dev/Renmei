using Microsoft.EntityFrameworkCore;
using Renmei.Infrastructure.Persistence;

namespace Renmei.Application.Tests.Support;

public sealed class TestDb
{
    private readonly DbContextOptions<ApplicationDbContext> _options =
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    public ApplicationDbContext NewContext() => new(_options);
}