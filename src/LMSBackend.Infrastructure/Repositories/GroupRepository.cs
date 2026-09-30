using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Groups;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LMSBackend.Infrastructure.Repositories;

public sealed class GroupRepository : IGroupRepository
{
    private readonly AppDbContext _context;
    public GroupRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<StudentGroup>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.StudentGroups.AsNoTracking().Where(g => !g.IsSoftDeleted)
            .OrderBy(g => g.Name).ToListAsync(cancellationToken);

    public Task<StudentGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _context.StudentGroups.SingleOrDefaultAsync(g => g.Id == id && !g.IsSoftDeleted, cancellationToken);

    public async Task<IReadOnlyList<StudentGroup>> GetForStudentAsync(Guid studentId, CancellationToken cancellationToken) =>
        await _context.GroupMemberships.AsNoTracking()
            .Where(m => m.StudentId == studentId && !m.Group.IsSoftDeleted)
            .Select(m => m.Group).OrderBy(g => g.Name).ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, Guid? exceptId, CancellationToken cancellationToken) =>
        _context.StudentGroups.AnyAsync(g => g.Name == name && (!exceptId.HasValue || g.Id != exceptId.Value), cancellationToken);

    public async Task AddAsync(StudentGroup group, CancellationToken cancellationToken)
    {
        _context.StudentGroups.Add(group);
    }

    public async Task<IReadOnlyList<GroupMembership>> GetMembershipsAsync(Guid groupId, CancellationToken cancellationToken) =>
        await _context.GroupMemberships.AsNoTracking().Include(m => m.Student)
            .Where(m => m.GroupId == groupId && !m.Student.IsSoftDeleted)
            .OrderBy(m => m.Student.Name).ToListAsync(cancellationToken);

    public Task<GroupMembership?> GetMembershipAsync(Guid groupId, Guid studentId, CancellationToken cancellationToken) =>
        _context.GroupMemberships.SingleOrDefaultAsync(m => m.GroupId == groupId && m.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<User>> GetStudentsAsync(Guid[] studentIds, CancellationToken cancellationToken) =>
        await _context.Users.AsNoTracking()
            .Where(u => studentIds.Contains(u.Id) && !u.IsSoftDeleted).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<GroupMembership>> GetMembershipsAsync(Guid groupId, Guid[] studentIds, CancellationToken cancellationToken) =>
        await _context.GroupMemberships
            .Where(m => m.GroupId == groupId && studentIds.Contains(m.StudentId)).ToListAsync(cancellationToken);

    public async Task AddMembershipsAsync(IEnumerable<GroupMembership> memberships, CancellationToken cancellationToken)
    {
        _context.GroupMemberships.AddRange(memberships);
    }

    public async Task RemoveMembershipsAsync(IEnumerable<GroupMembership> memberships, CancellationToken cancellationToken)
    {
        _context.GroupMemberships.RemoveRange(memberships);
    }


}
