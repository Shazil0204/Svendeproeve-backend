using LMSBackend.Application.DTOs.Groups;

namespace LMSBackend.Application.Abstractions.Groups;

public interface IGroupService
{
    Task<IReadOnlyList<GroupDto>> ListAsync(CancellationToken cancellationToken);
    Task<GroupDetailsDto> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<GroupDto>> ListForStudentAsync(Guid studentId, CancellationToken cancellationToken);
    Task<GroupDetailsDto> GetForStudentAsync(Guid id, Guid studentId, CancellationToken cancellationToken);
    Task<GroupDto> CreateAsync(string name, CancellationToken cancellationToken);
    Task<GroupDto> RenameAsync(Guid id, string name, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task AddStudentsAsync(Guid id, IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);
    Task RemoveStudentsAsync(Guid id, IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);
}
