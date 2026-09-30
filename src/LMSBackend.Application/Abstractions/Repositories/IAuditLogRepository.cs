namespace LMSBackend.Application.Abstractions.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(Domain.Entities.Auditing.AuditLog auditLog, CancellationToken cancellationToken = default);
}
