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
        DbContext? context = eventData.Context;

        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry>? entries = context.ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is not AuditLog &&
                e.State is EntityState.Added
                    or EntityState.Modified
                    or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            AuditAction action = entry.State switch
            {
                EntityState.Added => AuditAction.Created,
                EntityState.Deleted => AuditAction.Deleted,
                EntityState.Modified
                    when entry.Metadata.FindProperty("IsSoftDeleted") is not null &&
                        entry.Property("IsSoftDeleted").CurrentValue is true &&
                        entry.Property("IsSoftDeleted").OriginalValue is false
                    => AuditAction.Deleted,
                EntityState.Modified => AuditAction.Updated,
                _ => throw new InvalidOperationException()
            };

            string? entityType = entry.Metadata.ClrType.Name;

            Guid? entityId = null;

            Microsoft.EntityFrameworkCore.Metadata.IKey? primaryKey = entry.Metadata.FindPrimaryKey();

            if (primaryKey is not null && primaryKey.Properties.Count == 1)
            {
                object? keyValue = entry.Property(
                    primaryKey.Properties[0].Name).CurrentValue;

                if (keyValue is Guid id)
                    entityId = id;
            }

            AuditLog? auditLog = new AuditLog(
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