using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Domain.Entities.Auditing;
using LMSBackend.Domain.Enums.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LMSBackend.Infrastructure.Auditing;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditSaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = context.ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is not AuditLog &&
                e.State is EntityState.Added
                    or EntityState.Modified
                    or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var action = entry.State switch
            {
                EntityState.Added => AuditAction.Created,
                EntityState.Modified => AuditAction.Updated,
                EntityState.Deleted => AuditAction.Deleted,
                _ => throw new InvalidOperationException()
            };

            var entityType = entry.Metadata.ClrType.Name;

            Guid? entityId = null;

            var primaryKey = entry.Metadata.FindPrimaryKey();

            if (primaryKey is not null && primaryKey.Properties.Count == 1)
            {
                var keyValue = entry.Property(
                    primaryKey.Properties[0].Name).CurrentValue;

                if (keyValue is Guid id)
                    entityId = id;
            }

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                action,
                $"{entityType} {action.ToString().ToLowerInvariant()}",
                entityType,
                entityId);

            context.Set<AuditLog>().Add(auditLog);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}