namespace Renmei.Application.Abstractions;

public interface IClock { DateTimeOffset UtcNow { get; } }