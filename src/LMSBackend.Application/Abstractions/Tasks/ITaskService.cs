using LMSBackend.Application.DTOs.Tasks;

namespace LMSBackend.Application.Abstractions.Tasks;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> ListAsync(CancellationToken cancellationToken);
    Task<TaskDetailsDto> GetAsync(Guid taskId, CancellationToken cancellationToken);
    Task<TaskDto> CreateAsync(SaveTaskRequest request, CancellationToken cancellationToken);
    Task<TaskDto> UpdateAsync(Guid taskId, SaveTaskRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid taskId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskDto>> GetTasksBySubjectAsync(Guid subjectId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskAssignmentDto>> AssignAsync(Guid taskId, AssignTaskRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskObjectiveDto>> SetObjectivesAsync(Guid taskId, UpdateTaskObjectivesRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskAssignmentDto>> ListAssignmentsForStudentAsync(Guid studentId, CancellationToken cancellationToken);
    Task<TaskAssignmentDto> GetAssignmentAsync(Guid assignmentId, Guid? studentId, CancellationToken cancellationToken);
}

