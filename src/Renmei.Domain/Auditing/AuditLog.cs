namespace Renmei.Domain.Auditing;

public class AuditLog
{
    private AuditLog() { }

    public AuditLog(
        Guid? actorUserId,
        string action,
        string entityType,
        string entityId,
        string? detailsJson,
        string? ipAddress,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action is required.", nameof(action));
        if (string.IsNullOrWhiteSpace(entityType)) throw new ArgumentException("EntityType is required.", nameof(entityType));
        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException("EntityId is required.", nameof(entityId));

        ActorUserId = actorUserId;
        Action = action.Trim();
        EntityType = entityType.Trim();
        EntityId = entityId.Trim();
        DetailsJson = detailsJson;
        IpAddress = ipAddress;
        CreatedAt = createdAtUtc;
    }

    public long Id { get; private set; }
    public Guid? ActorUserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string EntityType { get; private set; } = null!;
    public string EntityId { get; private set; } = null!;
    public string? DetailsJson { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }
}