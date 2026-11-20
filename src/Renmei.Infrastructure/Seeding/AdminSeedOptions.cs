namespace Renmei.Infrastructure.Seeding;

public sealed class AdminSeedOptions
{
    public const string Section = "Seed:Admin";
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? DisplayName { get; init; }
}