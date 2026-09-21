using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Auditing;
using LMSBackend.Infrastructure.Data;

namespace LMSBackend.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _dbContext;

    public AuditLogRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<AuditLog>()
            .AddAsync(auditLog, cancellationToken);
    }
}
