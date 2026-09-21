using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Groups;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LMSBackend.Infrastructure.Repositories;

public sealed class GroupRepository(AppDbContext context) : IGroupRepository
{
    public async Task<IReadOnlyList<StudentGroup>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.StudentGroups.AsNoTracking().Where(g => !g.IsSoftDeleted)
            .OrderBy(g => g.Name).ToListAsync(cancellationToken);

    public Task<StudentGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.StudentGroups.SingleOrDefaultAsync(g => g.Id == id && !g.IsSoftDeleted, cancellationToken);

    public async Task<IReadOnlyList<StudentGroup>> GetForStudentAsync(Guid studentId, CancellationToken cancellationToken) =>
        await context.GroupMemberships.AsNoTracking()
            .Where(m => m.StudentId == studentId && !m.Group.IsSoftDeleted)
            .Select(m => m.Group).OrderBy(g => g.Name).ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, Guid? exceptId, CancellationToken cancellationToken) =>
        context.StudentGroups.AnyAsync(g => g.Name == name && (!exceptId.HasValue || g.Id != exceptId.Value), cancellationToken);

    public async Task AddAsync(StudentGroup group, CancellationToken cancellationToken)
    {
        context.StudentGroups.Add(group);
        await SaveAsync(cancellationToken);
    }

    public async Task UpdateAsync(StudentGroup group, CancellationToken cancellationToken)
    {
        context.StudentGroups.Update(group);
        await SaveAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GroupMembership>> GetMembershipsAsync(Guid groupId, CancellationToken cancellationToken) =>
        await context.GroupMemberships.AsNoTracking().Include(m => m.Student)
            .Where(m => m.GroupId == groupId && !m.Student.IsSoftDeleted)
            .OrderBy(m => m.Student.Name).ToListAsync(cancellationToken);

    public Task<GroupMembership?> GetMembershipAsync(Guid groupId, Guid studentId, CancellationToken cancellationToken) =>
        context.GroupMemberships.SingleOrDefaultAsync(m => m.GroupId == groupId && m.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<User>> GetStudentsAsync(Guid[] studentIds, CancellationToken cancellationToken) =>
        await context.Users.AsNoTracking()
            .Where(u => studentIds.Contains(u.Id) && !u.IsSoftDeleted).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<GroupMembership>> GetMembershipsAsync(Guid groupId, Guid[] studentIds, CancellationToken cancellationToken) =>
        await context.GroupMemberships
            .Where(m => m.GroupId == groupId && studentIds.Contains(m.StudentId)).ToListAsync(cancellationToken);

    public async Task AddMembershipsAsync(IEnumerable<GroupMembership> memberships, CancellationToken cancellationToken)
    {
        context.GroupMemberships.AddRange(memberships);
        await SaveAsync(cancellationToken);
    }

    public async Task RemoveMembershipsAsync(IEnumerable<GroupMembership> memberships, CancellationToken cancellationToken)
    {
        context.GroupMemberships.RemoveRange(memberships);
        await SaveAsync(cancellationToken);
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        try { await context.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgres)
        {
            switch (postgres.ConstraintName)
            {
                case "IX_StudentGroups_Name":
                    throw new ConflictException("A group with this name already exists.");
                case "PK_GroupMemberships":
                    throw new ConflictException("Student is already a member.");
                default:
                    throw;
            }
        }
    }
}
