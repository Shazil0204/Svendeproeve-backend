using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Auditing;
using LMSBackend.Domain.Enums.Auditing;

namespace LMSBackend.Application.Services.Auditing;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogAsync(
        Guid? userId,
        AuditAction action,
        string actionDescription,
        string entityType,
        Guid? entityId,
        CancellationToken cancellationToken = default)
    {
        AuditLog auditLog = new(
            userId,
            action,
            actionDescription,
            entityType,
            entityId);

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
    }
}
