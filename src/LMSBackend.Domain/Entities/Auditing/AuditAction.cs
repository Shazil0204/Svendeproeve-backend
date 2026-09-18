using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Auditing;

namespace LMSBackend.Domain.Entities.Auditing;

public class AuditLog
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public AuditAction Action { get; private set; }
    public string ActionDescription { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public User? User { get; private set; }

    private AuditLog() { }

    public AuditLog(
        Guid? userId,
        AuditAction action,
        string actionDescription,
        string entityType,
        Guid? entityId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Action = action;
        ActionDescription = actionDescription;
        EntityType = entityType;
        EntityId = entityId;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}