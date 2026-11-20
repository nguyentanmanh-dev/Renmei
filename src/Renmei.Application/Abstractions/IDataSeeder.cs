namespace Renmei.Application.Abstractions;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken ct = default);
}