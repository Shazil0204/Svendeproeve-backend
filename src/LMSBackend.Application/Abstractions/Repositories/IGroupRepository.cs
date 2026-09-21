using LMSBackend.Domain.Entities.Groups;
using LMSBackend.Domain.Entities.Users;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IGroupRepository
{
    Task<IReadOnlyList<StudentGroup>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<StudentGroup>> GetForStudentAsync(Guid studentId, CancellationToken cancellationToken);
    Task<StudentGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> NameExistsAsync(string name, Guid? exceptId, CancellationToken cancellationToken);
    Task AddAsync(StudentGroup group, CancellationToken cancellationToken);
    Task UpdateAsync(StudentGroup group, CancellationToken cancellationToken);
    Task<IReadOnlyList<GroupMembership>> GetMembershipsAsync(Guid groupId, CancellationToken cancellationToken);
    Task<GroupMembership?> GetMembershipAsync(Guid groupId, Guid studentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> GetStudentsAsync(Guid[] studentIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<GroupMembership>> GetMembershipsAsync(Guid groupId, Guid[] studentIds, CancellationToken cancellationToken);
    Task AddMembershipsAsync(IEnumerable<GroupMembership> memberships, CancellationToken cancellationToken);
    Task RemoveMembershipsAsync(IEnumerable<GroupMembership> memberships, CancellationToken cancellationToken);
}
