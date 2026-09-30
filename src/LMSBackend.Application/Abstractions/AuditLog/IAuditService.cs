using LMSBackend.Domain.Enums.Auditing;

namespace LMSBackend.Application.Abstractions.AuditLog;

public interface IAuditService
{
    Task LogAsync(Guid? userId, AuditAction action, string actionDescription, string entityType, Guid? entityId, CancellationToken cancellationToken = default);
}
